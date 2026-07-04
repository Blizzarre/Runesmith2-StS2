#region

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class FissionHammer : Runesmith2Card
{
    public FissionHammer() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(11, 2);
        WithCards(1);
        WithCalculatedVar("CalculatedEnhanceBy", 1,
            (c, _) => c.CombatState != null
                ? RunesmithHook.ModifyEnhanceAmount(c.CombatState, c.Owner, c.GetEnhance(), c, out var _)
                : c.GetEnhance(), 1);
        WithTip(RunesmithHoverTip.Enhance);
        WithTags(RunesmithTags.Hammer);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);

        var enhanceBy = this.GetEnhance() + DynamicVars["CalculatedEnhanceByBase"].IntValue;
        if (enhanceBy > 0)
        {
            var cards = (await CardSelectCmd.FromHand(choiceContext, Owner,
                new CardSelectorPrefs(RunesmithCardSelectorPrefs.EnhanceSelectionPrompt, DynamicVars.Cards.IntValue),
                c => c.CanEnhance(), this
            )).ToList();
            if (cards.Count != 0) await RunesmithCardCmd.Enhance(choiceContext, Owner, cards, play, enhanceBy);
        }
    }
}