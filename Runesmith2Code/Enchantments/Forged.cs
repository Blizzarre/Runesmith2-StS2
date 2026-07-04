#region

using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Enchantments;

public class Forged : Runesmith2Enchantment
{
    public override bool ShowAmount => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        RunesmithHoverTipFactory.Static(RunesmithHoverTip.Enhance)
    ];

    public override bool CanEnchant(CardModel card)
    {
        return card.CanEnhance() && base.CanEnchant(card);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Card.Owner && player.Creature.CombatState is { RoundNumber: <= 1 })
        {
            await RunesmithCardCmd.Enhance(choiceContext, player, Card, null, Amount, true);
            Status = EnchantmentStatus.Disabled;
        }
    }
}