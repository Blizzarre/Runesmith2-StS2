#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class CalmBeforeStormPower : Runesmith2Power
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return player != Owner.Player || AmountOnTurnStart == 0 ? count : count + Amount;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player)
        {
            Flash();
            var cards = PileType.Hand.GetPile(Owner.Player).Cards;
            await RunesmithCardCmd.Enhance(choiceContext, Owner.Player, cards.Where(c => c.CanEnhance()), null, 1);
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -Amount, null, null);
        }
    }
}