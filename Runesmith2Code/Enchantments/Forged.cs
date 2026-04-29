using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;

namespace Runesmith2.Runesmith2Code.Enchantments;

public class Forged : Runesmith2Enchantment
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Card.Owner && player.Creature.CombatState!.RoundNumber <= 1)
        {
            await RunesmithCardCmd.Enhance(choiceContext, player, Card, null, Amount, true);
        }
    }
}