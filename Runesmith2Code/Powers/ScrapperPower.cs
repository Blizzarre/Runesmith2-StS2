#region

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class ScrapperPower : Runesmith2Power, IAfterRuneBroken
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public Task AfterRuneBroken(PlayerChoiceContext choiceContext, Player player, RuneModel rune)
    {
        if (player != Owner.Player) return Task.CompletedTask;
        Flash();
        RuneCmd.ChargeAll(choiceContext, player, Amount);

        return Task.CompletedTask;
    }
}