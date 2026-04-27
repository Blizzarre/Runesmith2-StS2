#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class HammerAndChisel : Runesmith2Card
{
    public HammerAndChisel() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var hammer = CardFactory.GetDistinctForCombat(Owner,
                Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                    .Where(c => c.Tags.Contains(RunesmithTag.Hammer)), 1, Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
        var chisel = CardFactory.GetDistinctForCombat(Owner,
                Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
                    .Where(c => c.Tags.Contains(RunesmithTag.Chisel)), 1, Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
        if (hammer != null)
        {
            if (IsUpgraded)
            {
                hammer.UpgradeInternal();
            }

            hammer.EnergyCost.AddThisCombat(-1);
            await CardPileCmd.Add(hammer, PileType.Hand);
        }

        if (chisel != null)
        {
            if (IsUpgraded)
            {
                chisel.UpgradeInternal();
            }

            chisel.EnergyCost.AddThisCombat(-1);
            await CardPileCmd.Add(chisel, PileType.Hand);
        }
    }
}