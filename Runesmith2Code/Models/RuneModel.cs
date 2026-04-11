using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Nodes.Runes;

namespace Runesmith2.Runesmith2Code.Models;

public abstract class RuneModel : AbstractModel, ICustomModel
{

    public const string LocTable = "runes";

    private static readonly ModelId[] _validRunes = [
    //TODO
    ];
    
    private RuneModel _canonicalInstance;

    private Player? _owner;

    public abstract decimal PassiveVal { get; }
    
    public virtual decimal BreakVal => PassiveVal * 2;
    
    public abstract int ChargeVal { get; }
    
    public bool HasBeenRemovedFromState { get; private set; }

    public virtual int RemainingCharge => ChargeVal;

    public LocString Title => new(LocTable, Id.Entry + ".title");

    public LocString Description => new(LocTable, Id.Entry + ".description");

    public bool HasSmartDescription => LocString.Exists(LocTable, SmartDescriptionLocKey);

    private string SmartDescriptionLocKey => Id.Entry + ".smartDescription";
    
    public LocString SmartDescription => !HasSmartDescription ? Description : new LocString(LocTable, Id.Entry + ".smartDescription");
    
    public abstract (bool, bool) ShowTopLabel { get; }
    
    public abstract (decimal, decimal) TopValue { get; }

    public virtual (Color, Color, Color) TopLabelColor => NRune.DefaultFontColor;
    
    public virtual (Color, Color, Color) TopLabelBreakColor => NRune.BreakFontColor;
    
    public abstract (bool, bool) ShowBottomLabel { get; }
    
    public abstract (decimal, decimal) BottomValue { get; }
    
    public virtual (Color, Color, Color) BottomLabelColor => NRune.DefaultFontColor;
    
    public virtual (Color, Color, Color) BottomBreakColor => NRune.BreakFontColor;

    protected virtual string PassiveSfx => "";

    protected virtual string BreakSfx => "";

    protected virtual string CraftSfx => "";

    public static HoverTip EmptySlotHoverTipHoverTip => new(new LocString("runes", "RUNESMITH2-EMPTY_SLOT.title"),
        new LocString("runes", "RUNESMITH2-EMPTY_SLOT.description"));

    public HoverTip DumbHoverTip => RunesmithHoverTipFactory.CreateRuneHoverTip(this, Description);
    
    // Return the Recipe card that crafts this Rune.
    public abstract Runesmith2RecipeCard RecipeCard { get; }
    
    protected virtual IEnumerable<IHoverTip> ExtraHoverTips => [];
    
    public IEnumerable<IHoverTip> HoverTips
    {
        get
        {
            var list = ExtraHoverTips.ToList();
            if (HasSmartDescription && IsMutable)
            {
                var smartDescription = SmartDescription;
                smartDescription.Add("energyPrefix", Owner.Character.CardPool.Title);
                smartDescription.Add("Passive", PassiveVal);
                smartDescription.Add("Break", BreakVal);
                smartDescription.Add("Charge", ChargeVal);
                list.Add(RunesmithHoverTipFactory.CreateRuneHoverTip(this, smartDescription));
            }
            else
            {
                list.Add(DumbHoverTip);
            }

            return list;
        }    
    }
    
    private string IconPath => Id.Entry.ToLowerInvariant().RuneImagePath();
    
    private string SpritePath => Id.Entry.ToLowerInvariant().RuneScenePath();

    public CompressedTexture2D Icon => PreloadManager.Cache.GetCompressedTexture2D(IconPath);
    
    public abstract Color DarkenedColor { get; }
    
    private RuneModel CanonicalInstance
    {
        get => !IsMutable ? this : _canonicalInstance;
        set
        {
            AssertMutable();
            _canonicalInstance = value;
        }
    }
    
    public Player Owner
    {
        get
        {
            AssertMutable();
            return _owner;
        }
        set
        {
            AssertMutable();
            if (_owner != null && value != null && value != _owner)
            {
                throw new InvalidOperationException("Rune " + Id.Entry + " already has an owner.");
            }
            _owner = value;
        }
    }

    public CombatState CombatState => Owner.Creature.CombatState;

    public override bool ShouldReceiveCombatHooks => true;
    
    public event Action? Triggered;
    
    protected void PlayPassiveSfx()
    {
        if (PassiveSfx != "")
        {
            SfxCmd.Play(PassiveSfx);
        }
    }

    protected void PlayEvokeSfx()
    {
        if (BreakSfx != "")
        {
            SfxCmd.Play(BreakSfx);
        }
    }

    public void PlayChannelSfx()
    {
        if (CraftSfx != "")
        {
            SfxCmd.Play(CraftSfx);
        }
    }
    
    public Node2D CreateSprite()
    { 
        Node2D node2D = PreloadManager.Cache.GetScene(SpritePath).Instantiate<Node2D>();
        new MegaSprite(node2D.GetNode("SpineSkeleton")).GetAnimationState().SetAnimation("idle_loop");
        return node2D;
    }
    
    public RuneModel ToMutable(int initialAmount = 0)
    {
        AssertCanonical();
        var orbModel = (RuneModel)MutableClone();
        orbModel.CanonicalInstance = this;
        return orbModel;
    }
    
    public void Trigger()
    {
        this.Triggered?.Invoke();
    }
    
    public virtual Task BeforeTurnEndRuneTrigger(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task AfterTurnStartOrbTrigger(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }

    public virtual Task Passive(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task Break(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }

    // Note: Rune value shouldn't get modified after craft but using this just in case
    protected decimal ModifyRuneValue(decimal result)
    {
        return RunesmithHook.ModifyRuneValue(Owner.Creature.CombatState, Owner, result);
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        Triggered = null;
    }

    public void RemoveInternal()
    {
        HasBeenRemovedFromState = true;
    }
}