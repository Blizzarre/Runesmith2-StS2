#region

using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using Runesmith2.Runesmith2Code.Models;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Nodes.Runes;

[GlobalClass]
public partial class NRune : NClickableControl
{
    // TODO refactor this to Font color utils class
    public static readonly (Color, Color, Color) DefaultFontColor =
        (new Color("fff6e2"), new Color("00000040"), new Color("333333e6"));

    public static readonly (Color, Color, Color) BlueFontColor =
        (new Color("b7d4f2"), new Color("00000040"), new Color("283252e6"));

    public static readonly (Color, Color, Color) BreakFontColor = (new Color("5effff"), new Color("00000040"),
        new Color("143652e6"));

    public static readonly (Color, Color, Color) GreenFontColor = (StsColors.green, new Color("00000040"),
        StsColors.energyGreenOutline);

    private static readonly (Color, Color, Color) ChargeFontColor =
        (new Color("f4e8c7"), new Color("00000030"), new Color("554c36"));

    private static readonly Color NoChargeModulateColor = new("756b4c");

    private static readonly Color OverChargeModulateColor = new(1.164f, 1.164f, 1.164f);

    private CanvasGroup _outlineGroup = null!; // empty slot outline

    private AnimationPlayer _animationPlayer = null!;

    private Panel _chargePanel = null!;

    private HSeparator _chargeCross = null!;

    private Control _visualContainer = null!;

    private Control _labelContainer = null!;

    private MegaLabel _topLabel = null!;

    private MegaLabel _topBreakLabel = null!;

    private MegaLabel _bottomLabel = null!;

    private MegaLabel _bottomBreakLabel = null!;

    private MegaLabel _chargeLabel = null!;

    private readonly List<VSeparator> _chargeSeparators = [];

    private Control _bounds = null!;

    private CpuParticles2D _flashParticle = null!;

    private GpuParticles2D _chargeSparks = null!;

    private NSelectionReticle _selectionReticle = null!;

    private bool _isLocal;

    private NRuneVisuals? _sprite;

    private Tween? _curTween;

    public RuneModel? Model { get; private set; }

    private static string ScenePath => RunesmithResource.NRunePath;

    public static NRune Create(bool isLocal)
    {
        var nRune = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NRune>();
        nRune._isLocal = isLocal;
        return nRune;
    }

    public static NRune Create(bool isLocal, RuneModel? model)
    {
        var nRune = Create(isLocal);
        nRune.Model = model;

        return nRune;
    }

    public override void _Ready()
    {
        ConnectSignals();
        _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        _animationPlayer.Play("loop");
        _animationPlayer.Advance(GD.RandRange(0, _animationPlayer.CurrentAnimationLength));

        _outlineGroup = GetNode<CanvasGroup>("%OutlineGroup");
        _visualContainer = GetNode<Control>("%VisualContainer");
        _labelContainer = GetNode<Control>("%LabelContainer");
        _flashParticle = GetNode<CpuParticles2D>("%Flash");
        _chargeSparks = GetNode<GpuParticles2D>("%ChargeSparks");
        _chargePanel = GetNode<Panel>("%ChargePanel");
        _chargeCross = GetNode<HSeparator>("%ChargeCross");
        for (var i = 1; i < 9; i++) _chargeSeparators.Add(GetNode<VSeparator>($"%Separator{i}"));

        _chargePanel.Position = new Vector2(0, _chargePanel.Position.Y);
        _chargePanel.Size = new Vector2(0, _chargePanel.Size.Y);

        _bounds = GetNode<Control>("%Bounds");

        // Create base game's nodes and scenes
        _topLabel = CreateLabel(Model?.TopLabelColor ?? DefaultFontColor);
        _topBreakLabel = CreateLabel(Model?.TopLabelBreakColor ?? DefaultFontColor);
        _bottomLabel = CreateLabel(Model?.BottomLabelColor ?? DefaultFontColor);
        _bottomBreakLabel = CreateLabel(Model?.BottomBreakColor ?? DefaultFontColor);
        _chargeLabel = CreateLabel(ChargeFontColor);

        _selectionReticle = BaseSceneIndex.SelectionReticleScene.Instantiate<NSelectionReticle>();

        _labelContainer.AddChildSafely(_topLabel);
        _labelContainer.AddChildSafely(_topBreakLabel);
        _labelContainer.AddChildSafely(_bottomLabel);
        _labelContainer.AddChildSafely(_bottomBreakLabel);

        this.AddChildSafely(_chargeLabel);
        _chargeLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _chargeLabel.VerticalAlignment = VerticalAlignment.Center;
        _chargeLabel.Position = new Vector2(-50, 20);
        _chargeLabel.Size = new Vector2(100, 40);

        this.AddChildSafely(_selectionReticle);
        _selectionReticle.Size = new Vector2(90, 90);
        _selectionReticle.Position = new Vector2(-45, -45);
        _selectionReticle.PivotOffset = new Vector2(45, 45);

        if (Model == null) CreateTween().TweenProperty(_outlineGroup, "scale", Vector2.One, 0.25).From(Vector2.Zero);

        if (_isLocal) Scale *= 0.85f;

        UpdateVisuals(false);
    }

    private static MegaLabel CreateLabel((Color, Color, Color) fontColor)
    {
        var label = new MegaLabel();
        label.MaxFontSize = 24;
        label.AutoSizeEnabled = false;
        label.HorizontalAlignment = HorizontalAlignment.Left;
        label.VerticalAlignment = VerticalAlignment.Top;
        label.AddThemeColorOverride("font_color", fontColor.Item1);
        label.AddThemeColorOverride("font_shadow_color", fontColor.Item2);
        label.AddThemeColorOverride("font_outline_color", fontColor.Item3);
        label.AddThemeConstantOverride("shadow_offset_x", 3);
        label.AddThemeConstantOverride("shadow_offset_y", 3);
        label.AddThemeConstantOverride("outline_size", 13);
        label.AddThemeConstantOverride("shadow_outline_size", 0);
        label.AddThemeFontOverride("font", BaseResourceIndex.FontKreonBoldShared);
        label.AddThemeFontSizeOverride("font_size", 24);
        label.Text = "";

        return label;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        if (Model == null) return;
        Model.Triggered += Trigger;
        Model.Charged += Charged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (Model == null) return;
        Model.Triggered -= Trigger;
        Model.Charged -= Charged;
    }

    public void ReplaceRune(RuneModel? model)
    {
        _sprite?.QueueFreeSafely();
        _sprite = null;
        Model = model;
        UpdateVisuals(false);
    }


    public void UpdateVisuals(bool isBreaking)
    {
        if (!IsNodeReady() || !CombatManager.Instance.IsInProgress) return;

        if (Model == null)
        {
            _sprite?.QueueFreeSafely();
            _topLabel.Visible = false;
            _topBreakLabel.Visible = false;
            _bottomLabel.Visible = false;
            _bottomBreakLabel.Visible = false;
            _outlineGroup.Visible = _isLocal;
            _chargePanel.Visible = false;
            _flashParticle.Visible = false;
            return;
        }

        if (_sprite == null)
        {
            _sprite = Model.CreateSprite();
            _visualContainer.AddChildSafely(_sprite);
            _sprite.Position = Vector2.Zero;
            _curTween?.Kill();
            _curTween = CreateTween();
            _curTween.TweenProperty(_sprite, "scale", Vector2.One, 0.5).From(Vector2.Zero)
                .SetTrans(Tween.TransitionType.Back)
                .SetEase(Tween.EaseType.Out);
        }

        _outlineGroup.Visible = false;
        _chargePanel.Visible = _isLocal;
        _labelContainer.Visible = _isLocal;
        if (!_isLocal) Modulate = Model.DarkenedColor;

        if (isBreaking)
        {
            _topBreakLabel.Visible = Model.ShowTopLabel.Item2;
            var topText = Model.TopTextAppend.Item2;
            if (Model.TopValue.Item2 >= 0)
                topText = Model.TopValue.Item2.ToString("0") + topText;
            _topBreakLabel.SetTextAutoSize(topText);

            _bottomBreakLabel.Visible = Model.ShowBottomLabel.Item2;
            var bottomText = Model.BottomTextAppend.Item2;
            if (Model.BottomValue.Item2 >= 0)
                bottomText = Model.BottomValue.Item2.ToString("0") + bottomText;
            _bottomBreakLabel.SetTextAutoSize(bottomText);

            _topLabel.Visible = false;
            _bottomLabel.Visible = false;
        }
        else
        {
            _topLabel.Visible = Model.ShowTopLabel.Item1;
            var topText = Model.TopTextAppend.Item1;
            if (Model.TopValue.Item1 >= 0)
                topText = Model.TopValue.Item1.ToString("0") + topText;
            _topLabel.SetTextAutoSize(topText);

            _bottomLabel.Visible = Model.ShowBottomLabel.Item1;
            var bottomText = Model.BottomTextAppend.Item1;
            if (Model.BottomValue.Item1 >= 0)
                bottomText = Model.BottomValue.Item1.ToString("0") + bottomText;
            _bottomLabel.SetTextAutoSize(bottomText);

            _topBreakLabel.Visible = false;
            _bottomBreakLabel.Visible = false;
        }

        _sprite.SetChargeStatus(Model.ChargeVal <= 0, Model.DarkenedColor);

        UpdateChargeDisplay(Model.ChargeVal);
    }

    private static readonly int[] ChargeSizeSequence = [24, 40, 60, 75, 80, 85, 90, 95, 98, 99];

    private void UpdateChargeDisplay(int charge)
    {
        var tween = GetTree().CreateTween();
        tween.SetParallel();

        Color modulateColor;
        switch (charge)
        {
            case <= 0:
                _chargeLabel.Visible = false;
                modulateColor = NoChargeModulateColor;
                _chargeCross.Visible = false;
                break;
            case > 9:
                _chargeLabel.Visible = true;
                _chargeLabel.SetTextAutoSize(charge.ToString());
                modulateColor = OverChargeModulateColor;
                _chargeCross.Visible = true;
                break;
            default:
                _chargeLabel.Visible = false;
                modulateColor = Colors.White;
                _chargeCross.Visible = true;
                break;
        }

        _chargeLabel.Visible = _isLocal && _chargeLabel.Visible;

        tween.TweenProperty(_chargePanel, "modulate", modulateColor, 0.25).FromCurrent()
            .FromCurrent().SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);

        var panelWidth = ChargeSizeSequence[Math.Clamp(charge, 0, 9)];

        tween.TweenProperty(_chargePanel, "size:x", panelWidth, 0.25)
            .FromCurrent().SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
        tween.Parallel().TweenProperty(_chargePanel, "position:x", -panelWidth / 2, 0.25)
            .FromCurrent().SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);

        for (var i = 1; i < 9; i++) _chargeSeparators[i - 1].Visible = i < charge;
    }

    private void Trigger()
    {
        _flashParticle.Restart();

        _sprite?.OnTrigger();
    }

    private void Charged(bool isPositive)
    {
        if (isPositive) CreateTween().TweenCallback(Callable.From(PlayCharged)).SetDelay(0.15);

        UpdateVisuals(false);
    }

    private void PlayCharged()
    {
        if (_chargeSparks.Emitting)
        {
            var newEmitter = (GpuParticles2D)_chargeSparks.Duplicate();
            newEmitter.Emitting = true;
            _chargeSparks.AddSibling(newEmitter);
            newEmitter.Restart();
            newEmitter.Connect("finished", Callable.From(() => newEmitter.QueueFree()));
        }
        else
        {
            _chargeSparks.Restart();
        }
    }

    public void OnBreak()
    {
        _sprite?.OnBreak();

        var tween = CreateTween().SetParallel();
        tween.TweenProperty(_chargePanel, "modulate", Colors.Transparent, 0.25);
        tween.TweenProperty(_labelContainer, "modulate", Colors.Transparent, 0.25);
        tween.TweenProperty(_chargeLabel, "modulate", Colors.Transparent, 0.25);
    }

    protected override void OnFocus()
    {
        if (Model == null && !_isLocal) return;
        var hoverTips = Model?.HoverTips ?? new List<IHoverTip> { RuneModel.EmptySlotHoverTipHoverTip };
        var nHoverTipSet = NHoverTipSet.CreateAndShow(_bounds, hoverTips, HoverTip.GetHoverTipAlignment(_bounds));
        nHoverTipSet?.SetFollowOwner();
        _labelContainer.Visible = true;
        Modulate = Colors.White;
        if (!NControllerManager.Instance?.IsUsingDirectionalNavigation ?? false)
            return;
        _selectionReticle.OnSelect();
    }

    protected override void OnUnfocus()
    {
        _labelContainer.Visible = _isLocal;
        if (Model != null) Modulate = _isLocal ? Colors.White : Model.DarkenedColor;

        NHoverTipSet.Remove(_bounds);
        _selectionReticle.OnDeselect();
    }
}