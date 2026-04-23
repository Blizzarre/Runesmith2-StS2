using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Entities.Runes;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Models;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Nodes.Runes;

[GlobalClass]
public partial class NRuneManager : Control
{
    private Control _runeContainer;

    private readonly List<NRune> _runes = new();
    
    private NCreature _creatureNode;

    private const float Radius = 330f;
    
    private const float FanAngle = 100f;

    private const float AngleOffset = 10f;
    
    private const float TweenFadeDuration = 0.45f;
    
    private Tween? _curTween;

    private static readonly Vector2 CenterOffset = new(-100f, -70f);
    
    private static string ScenePath => RunesmithResource.NRuneManagerPath;
    
    public bool IsLocal { get; private set; }
    
    private Player Player => _creatureNode.Entity.Player;
    
    public Control DefaultFocusOwner
    {
        get
        {
            if (_runes.Count <= 0 || _runes.First().Model == null)
            {
                return _creatureNode.Hitbox;
            }
            return _runes.First();
        }
    }

    public override void _Ready()
    {
        _runeContainer = GetNode<Control>("%Runes");
    }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
        CombatManager.Instance.CombatSetUp += OnCombatSetup;
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
        CombatManager.Instance.CombatSetUp -= OnCombatSetup;
    }
    
    public static NRuneManager Create(NCreature creature, bool isLocal)
    {
        if (creature.Entity.Player == null)
        {
            throw new InvalidOperationException("NRuneManager can only be applied to player creatures");
        }
        var nRuneManager = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NRuneManager>();
        nRuneManager._creatureNode = creature;
        nRuneManager.IsLocal = isLocal;
        return nRuneManager;
    }
    
    private void OnCombatSetup(CombatState _)
    {
        if (!Player.Creature.IsAlive || Player.PlayerCombatState == null) return;
        if (Player.PlayerCombatState.AllCards.Any(c => c is Runesmith2RecipeCard))
        {
            AddSlotAnim();
        }
    }
    
    private void OnCombatStateChanged(CombatState _)
    {
        UpdateVisuals(RuneBreakType.None);
    }

    private void AddSlotAnim()
    {
        var emptyRune = _runes.FirstOrDefault(n => n.Model == null);
        if (emptyRune != null) return; // There's empty slot already
        
        var nonEmptyCount = _runes.Count(n => n.Model != null);
        if (nonEmptyCount >= RuneQueue.MaxCapacity) return; // Full of non-empty slot already
        
        var nRune = NRune.Create(LocalContext.IsMe(Player));
        var count = _runes.Count;
        _runeContainer.AddChildSafely(nRune);
        _runes.Add(nRune);
        nRune.Position = GetRunePosition(count);

        TweenLayout();
        UpdateControllerNavigation();
    }

    public void AddRuneAnim()
    {
        if (Player.PlayerCombatState == null) return;
        var queue = RunesmithField.RunesmithCombatState[Player.PlayerCombatState]?.RuneQueue;
        if (queue == null) return;
        var runeModel = queue.Runes.Count > 0 ? queue.Runes[^1] : null; // Get last (should be the newly added one)
        if (_runes.Count >= RuneQueue.MaxCapacity) return; // cannot add rune to full RuneQueue
        var emptyRune = _runes.FirstOrDefault(n => n.Model == null);
        var newRune = NRune.Create(LocalContext.IsMe(Player), runeModel);
        if (emptyRune == null)
        {
            // No need to replace the empty slot. Just add the Rune at the proper position.
            var position = GetRunePosition(_runes.Count);
            this.AddChildSafely(newRune);
            _runes.Add(newRune);
            newRune.Position = position;
        }
        else
        {
            emptyRune.AddSibling(newRune);
            _runes.Insert(_runes.IndexOf(emptyRune), newRune);
            newRune.Position = emptyRune.Position;
            _runeContainer.RemoveChildSafely(emptyRune);
            _runes.Remove(emptyRune);
            emptyRune.QueueFreeSafely();
        }
        
        AddSlotAnim();
        
        TweenLayout();
        UpdateControllerNavigation();
    }

    public void BreakRuneAnim(RuneModel rune)
    {
        var breakRune = _runes.Last(n => n.Model == rune);
        var tween = CreateTween();
        _runes.Remove(breakRune);
        
        var emptyRune = _runes.FirstOrDefault(n => n.Model == null);
        if (emptyRune != null)
        {
            tween.TweenProperty(emptyRune, "modulate:a", 0, TweenFadeDuration);
            tween.Parallel().TweenProperty(breakRune, "modulate:a", 0, TweenFadeDuration);
            tween.Chain().TweenCallback(Callable.From(emptyRune.QueueFreeSafely));
            tween.Parallel().TweenCallback(Callable.From(breakRune.QueueFreeSafely));
            _runes.Remove(emptyRune);
        }
        else
        {
            tween.TweenProperty(breakRune, "modulate:a", 0, TweenFadeDuration);
            tween.Chain().TweenCallback(Callable.From(breakRune.QueueFreeSafely));
        }
        
        var newEmptyRune = NRune.Create(LocalContext.IsMe(Player));
        _runeContainer.AddChildSafely(newEmptyRune);
        var position = GetRunePosition(_runes.Count);
        _runes.Add(newEmptyRune);
        newEmptyRune.Position = position;
        if (breakRune.HasFocus())
        {
            _creatureNode.Hitbox.TryGrabFocus();
        }
        TweenLayout();
        UpdateControllerNavigation();
    }

    private void UpdateControllerNavigation()
    {
        for (var i = 0; i < _runes.Count; i++)
        {
            var rune = _runes[i];
            var path = i <= 0 ? _runes[^1].GetPath() : _runes[i - 1].GetPath();
            rune.FocusNeighborRight = path;
            _runes[i].FocusNeighborLeft = i < _runes.Count - 1 ? _runes[i + 1].GetPath() : _runes[0].GetPath();
            _runes[i].FocusNeighborTop = _runes[i].GetPath();
            _runes[i].FocusNeighborBottom = _creatureNode.Hitbox.GetPath();
        }
        _creatureNode.UpdateNavigation();
    }

    private void TweenLayout()
    {
        _curTween?.Kill();
        _curTween = CreateTween().SetParallel();
        for (var i = 0; i < _runes.Count; i++)
        {
            var position = GetRunePosition(i);
            _curTween.TweenProperty(_runes[i], "position", position, 0.5).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
        }
    }
    
    private Vector2 GetRunePosition(int index)
    {
        var radius = Radius;
        if (!IsLocal)
        {
            radius *= 0.75f;
        }
        const float angleStep = FanAngle / (RuneQueue.MaxCapacity - 1);
        var angle = float.DegreesToRadians(-angleStep * index - AngleOffset); // neg angle is counter-clockwise
        return new Vector2(radius, 0f).Rotated(angle) + CenterOffset;
    }
    
    public void UpdateVisuals(RuneBreakType breakType)
    {
        foreach (var rune in _runes)
        {
            rune.UpdateVisuals(false);
        }
        switch (breakType)
        {
            case RuneBreakType.Oldest:
                _runes.FirstOrDefault()?.UpdateVisuals(true);
                break;
            case RuneBreakType.Newest:
                _runes.LastOrDefault()?.UpdateVisuals(true);
                break;
            case RuneBreakType.All:
            {
                foreach (var rune2 in _runes)
                {
                    rune2.UpdateVisuals(true);
                }
                break;
            }
            case RuneBreakType.None:
                break;
        }
    }

    public void ClearRunes()
    {
        _curTween?.Kill();
        if (_runes.Count == 0)
        {
            return;
        }

        _curTween = CreateTween();
        foreach (var rune in _runes)
        {
            _curTween.Parallel().TweenProperty(rune, "position", Vector2.Zero, 1.0).SetEase(Tween.EaseType.InOut)
                .SetTrans(Tween.TransitionType.Sine);
            _curTween.Parallel().TweenProperty(rune, "modulate:a", 0, 0.25);
        }
        foreach (var rune2 in _runes)
        {
            _curTween.Chain().TweenCallback(Callable.From(rune2.QueueFreeSafely));
        }
        _runes.Clear();
    }
}