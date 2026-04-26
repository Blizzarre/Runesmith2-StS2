using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using Runesmith2.Runesmith2Code.Character;
using Runesmith2.Runesmith2Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Models;
using Runesmith2.Runesmith2Code.Structs;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Cards;

[Pool(typeof(Runesmith2CardPool))]
public abstract class Runesmith2Card(int cost, CardType type, CardRarity rarity, TargetType target) :
    ConstructedCardModel(cost, type, rarity, target)
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
        }
    }

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
        }
    }

    public override string BetaPortraitPath
    {
        get
        {
            var path = $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : "beta/card.png".CardImagePath();
        }
    }

    protected void WithTip(RunesmithHoverTip runesmithTip)
    {
        switch (runesmithTip)
        {
            case RunesmithHoverTip.Elements:
                WithTip(new TooltipSource(_ => RunesmithHoverTipFactory.CreateElementsHoverTip()));
                break;
            default:
                WithTip(new TooltipSource(_ => RunesmithHoverTipFactory.Static(runesmithTip)));
                break;
        }
    }

    protected void WithRuneTip<T>() where T : RuneModel
    {
        WithTip(new TooltipSource(_ => RunesmithHoverTipFactory.FromRune<T>()));
    }

    public virtual decimal EnhanceMultiplier => 1m;

    public event Action? ElementsCostChanged;

    public void InvokeElementsCostChanged()
    {
        ElementsCostChanged?.Invoke();
    }

    private bool _elementsCostSet;

    public virtual Elements CanonicalElementsCost => new(-1, -1, -1);

    public List<TemporaryCardCost> _temporaryElementsCosts = [];

    public TemporaryCardCost? TemporaryElementsCost => _temporaryElementsCosts.LastOrDefault();

    public Elements BaseElementsCost
    {
        get
        {
            if (!IsMutable) return CanonicalElementsCost;

            if (_elementsCostSet) return field;

            field = CanonicalElementsCost;
            _elementsCostSet = true;
            return field;
        }
        private set
        {
            AssertMutable();
            field = value;
            _elementsCostSet = true;
        }
    }

    public virtual Elements CurrentElementsCost
    {
        get
        {
            var tempCost = TemporaryElementsCost?.Cost;
            if (!tempCost.HasValue || (tempCost == 0 && BaseElementsCost.Total < 0)) return BaseElementsCost;
            return new Elements(tempCost.Value);
        }
    }

    // DeepCloneFields
    protected override void DeepCloneFields()
    {
        base.DeepCloneFields();
        _temporaryElementsCosts = _temporaryElementsCosts.ToList();
    }

    // AfterCloned
    protected override void AfterCloned()
    {
        base.AfterCloned();
        ElementsCostChanged = null;
    }

    // SetToFreeThisTurn - patch done
    // SetToFreeThisCombat - patch done
    // SetStarCostUntilPlayed - unused

    // SetStarCostThisTurn
    public void SetElementsCostThisTurn(int cost)
    {
        AddTemporaryElementsCost(TemporaryCardCost.ThisTurn(cost));
    }

    // SetStarCostThisCombat
    public void SetElementsCostThisCombat(int cost)
    {
        AddTemporaryElementsCost(TemporaryCardCost.ThisCombat(cost));
    }

    // GetStarCostThisCombat - unused

    // AddTemporaryStarCost
    private void AddTemporaryElementsCost(TemporaryCardCost cost)
    {
        AssertMutable();
        _temporaryElementsCosts.Add(cost);
        ElementsCostChanged?.Invoke();
    }

    // UpgradeStarCostBy - unused

    // GetStarCostWithModifiers - todo patch usages
    public Elements GetElementsCostWithModifiers()
    {
        if (Pile != null && Pile.IsCombatPile && CombatState != null)
            return RunesmithHook.ModifyElementsCost(CombatState, this, CurrentElementsCost);

        return CurrentElementsCost;
    }

    // CostsEnergyOrStars - patch done

    // EndOfTurnCleanup - patch done

    // SpendResources - patch done

    // SpendStars
    public async Task SpendElements(Elements amount)
    {
        if (amount.Total > 0 && Owner.PlayerCombatState != null)
        {
            var runesmithCombatState = Owner.PlayerCombatState.Runesmith();
            if (runesmithCombatState != null)
            {
                runesmithCombatState.LoseElements(amount);
                await RunesmithHook.AfterElementsSpent(Owner.Creature.CombatState, amount, Owner);
            }
        }
    }

    // OnPlayWrapper - patch done

    // DowngradeInternal - patch set base cost (not really needed)
    
    protected bool HasRune()
    {
        if (IsInCombat) return Owner.PlayerCombatState?.RuneQueue()?.HasAny() ?? false;

        return false;
    }
}