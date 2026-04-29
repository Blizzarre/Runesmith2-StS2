#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Character;
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

        var cardPool = ModelDb.CardPool<Runesmith2CardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint).ToList();

        var hammer = CardFactory.GetDistinctForCombat(Owner,
                cardPool.Where(c => c.Tags.Contains(RunesmithEnum.Hammer)), 1, Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
        var chisel = CardFactory.GetDistinctForCombat(Owner,
                cardPool.Where(c => c.Tags.Contains(RunesmithEnum.Chisel)), 1, Owner.RunState.Rng.CombatCardGeneration)
            .FirstOrDefault();
        if (hammer != null)
        {
            if (IsUpgraded) CardCmd.Upgrade(hammer);

            hammer.EnergyCost.AddThisCombat(-1);
            await CardPileCmd.Add(hammer, PileType.Hand);
        }

        if (chisel != null)
        {
            if (IsUpgraded) CardCmd.Upgrade(chisel);

            chisel.EnergyCost.AddThisCombat(-1);
            await CardPileCmd.Add(chisel, PileType.Hand);
        }
    }
}