#region

using BaseLib.Abstracts;
using BaseLib.Audio;
using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Character;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Nodes.Runes;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Models;

public abstract class RuneModel : AbstractModel, ICustomModel
{
    public const string LocTable = "runes";

    public virtual decimal PassiveVal { get; set; }

    public virtual bool UsePotency => false;

    public virtual decimal CalculatedPassiveVal => PassiveVal;

    public virtual decimal BreakVal => PassiveVal * 2;

    public virtual decimal CalculatedBreakVal => BreakVal;

    public virtual int ChargeVal { get; set; }

    public virtual bool IsUpgradeable => false;

    public bool Upgraded { get; private set; }

    public abstract ChargeDepletionType ChargeDepletion { get; }

    public bool HasBeenRemovedFromState { get; private set; }

    public virtual int RemainingCharge => ChargeVal;

    public LocString Title =>
        Upgraded ? new LocString(LocTable, Id.Entry + ".titleUpgraded") : new LocString(LocTable, Id.Entry + ".title");

    public LocString Description => Upgraded
        ? new LocString(LocTable, Id.Entry + ".descriptionUpgraded")
        : new LocString(LocTable, Id.Entry + ".description");

    public bool HasSmartDescription => LocString.Exists(LocTable, SmartDescriptionLocKey);

    private string SmartDescriptionLocKey => Id.Entry + ".smartDescription";

    public LocString SmartDescription =>
        !HasSmartDescription ? Description : new LocString(LocTable, Id.Entry + ".smartDescription");

    // TODO Refactor text stuff to be in one class

    public virtual (bool, bool) ShowTopLabel => (false, false);

    public virtual (decimal, decimal) TopValue => (PassiveVal, BreakVal);

    public virtual (string, string) TopTextAppend => ("", "");

    public virtual (Color, Color, Color) TopLabelColor => NRune.DefaultFontColor;

    public virtual (Color, Color, Color) TopLabelBreakColor => NRune.BreakFontColor;

    public virtual (bool, bool) ShowBottomLabel => (true, true);

    public virtual (decimal, decimal) BottomValue => (PassiveVal, BreakVal);

    public virtual (string, string) BottomTextAppend => ("", "");

    public virtual (Color, Color, Color) BottomLabelColor => NRune.DefaultFontColor;

    public virtual (Color, Color, Color) BottomBreakColor => NRune.BreakFontColor;

    public virtual bool CanPassive => ChargeVal > 0;

    protected virtual ModSound? EffectSfx => RunesmithModSounds.RunePassiveSfx;

    protected virtual ModSound? CraftSfx => RunesmithModSounds.RuneCraftSfx;

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
                smartDescription.Add("energyPrefix", GetRuneOwnerPool().EnergyColorName);
                smartDescription.Add("Passive", PassiveVal);
                smartDescription.Add("CalculatedPassive", CalculatedPassiveVal);
                smartDescription.Add("Break", BreakVal);
                smartDescription.Add("CalculatedBreak", CalculatedBreakVal);
                smartDescription.Add("Charge", ChargeVal);
                smartDescription.Add("IfUpgraded", Upgraded);
                list.Add(RunesmithHoverTipFactory.CreateRuneHoverTip(this, smartDescription));
                var chargeLocKey = ChargeDepletion switch
                {
                    ChargeDepletionType.None => "RUNESMITH2-CHARGE-NONE",
                    ChargeDepletionType.StartTurn => "RUNESMITH2-CHARGE-TURN-START",
                    ChargeDepletionType.EndTurn => "RUNESMITH2-CHARGE-TURN-END",
                    _ => throw new ArgumentOutOfRangeException()
                };
                list.Add(RunesmithHoverTipFactory.Static(chargeLocKey, new DynamicVar("Charge", ChargeVal)));
            }
            else
            {
                list.Add(DumbHoverTip);
            }

            return list;
        }
    }

    private IPoolModel GetRuneOwnerPool()
    {
        return IsMutable ? Owner.Character.CardPool : ModelDb.CardPool<Runesmith2CardPool>();
    }

    private string IconPath => Id.Entry.RemovePrefix().ToLowerInvariant().RuneImagePath();

    private string SpritePath => Id.Entry.RemovePrefix().ToLowerInvariant().RuneScenePath();

    public CompressedTexture2D Icon => PreloadManager.Cache.GetCompressedTexture2D(IconPath);

    public virtual Color DarkenedColor => new("a0a0a0");

    private RuneModel? CanonicalInstance
    {
        get => !IsMutable ? this : field;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    private Player? _owner;

    public Player Owner
    {
        get
        {
            AssertMutable();
            return _owner ?? throw new Exception("Rune " + Id.Entry + " does not have an owner.");
        }
        set
        {
            AssertMutable();
            if (_owner != null && value != null && value != _owner)
                throw new InvalidOperationException("Rune " + Id.Entry + " already has an owner.");

            _owner = value;
        }
    }

    public void TransferOwner(Player newOwner)
    {
        AssertMutable();
        _owner = newOwner;
    }

    protected ICombatState? CombatState => Owner.Creature.CombatState;

    public override bool ShouldReceiveCombatHooks => true;

    public event Action? Triggered;

    public event Action<bool>? Charged;

    protected void PlayPassiveSfx()
    {
        EffectSfx?.Play(pitchVariation: 0.07f);
    }

    public void PlayCraftedSfx()
    {
        CraftSfx?.Play(pitchVariation: 0.07f);
    }

    public NRuneVisuals CreateSprite()
    {
        var runeVisuals = PreloadManager.Cache.GetScene(SpritePath).Instantiate<NRuneVisuals>();
        // new MegaSprite(node2D.GetNode("SpineSkeleton")).GetAnimationState().SetAnimation("idle_loop");
        return runeVisuals;
    }

    public RuneModel ToMutable()
    {
        AssertCanonical();
        var runeModel = (RuneModel)MutableClone();
        runeModel.CanonicalInstance = this;
        return runeModel;
    }

    public void Trigger()
    {
        Triggered?.Invoke();
    }

    // These triggers should return if it was triggerred or not
    public virtual Task<bool> BeforeTurnEndEarlyRuneTrigger(PlayerChoiceContext choiceContext)
    {
        return Task.FromResult(false);
    }

    public virtual Task<bool> BeforeTurnEndRuneTrigger(PlayerChoiceContext choiceContext)
    {
        return Task.FromResult(false);
    }

    public virtual Task<bool> SetupTurnStartRuneTrigger(PlayerChoiceContext choiceContext)
    {
        return Task.FromResult(false);
    }

    public virtual Task Passive(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }

    public virtual Task Break(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }

    public RuneModel CreateClone()
    {
        AssertMutable();
        var clonedRune = (RuneModel)ClonePreservingMutability();
        return clonedRune;
    }

    // Note: Rune value shouldn't get modified after craft but having this just in case
    protected decimal ModifyRuneValue(decimal result)
    {
        if (Owner.Creature.CombatState == null) return result;
        return RunesmithHook.ModifyRuneValue(Owner.Creature.CombatState, Owner, result);
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        Triggered = null;
        Charged = null;
    }

    public void RemoveInternal()
    {
        HasBeenRemovedFromState = true;
    }

    public void UseCharge()
    {
        ModifyCharge(-1);
    }

    public void ModifyCharge(int amount)
    {
        ChargeVal = Math.Max(0, ChargeVal + amount);
        Charged?.Invoke(amount > 0);
    }

    public void SetCharge(int amount)
    {
        ChargeVal = Math.Max(0, amount);
        Charged?.Invoke(amount > 0);
    }

    public void Upgrade()
    {
        if (IsUpgradeable) Upgraded = true;
    }

    protected List<Creature> GetHittableCreatures()
    {
        return CombatState?.GetOpponentsOf(Owner.Creature).Where(e => e.IsHittable).ToList() ?? [];
    }
}