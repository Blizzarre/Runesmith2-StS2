#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class GrindstonePower : Runesmith2Power
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private CardModel? _ownerCard;

    public void SetOwnerCard(CardModel card)
    {
        _ownerCard = card;
    }

    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Player == null) return;
        var card = cardPlay.Card;
        if (card == _ownerCard)
        {
            // Prevent Grindstone from upgrading itself immediately after play.
            _ownerCard = null;
            return;
        }
        if (card.IsUpgradable)
        {
            CardCmd.Upgrade(card);
            Flash();
        }
        else if (card.CanEnhance())
        {
            await RunesmithCardCmd.Enhance(choiceContext, Owner.Player, card, cardPlay, 1);
            Flash();
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await PowerCmd.Decrement(this);
    }
}