#region

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IAfterRuneBroken
{
    public Task AfterRuneBroken(PlayerChoiceContext choiceContext, Player player, RuneModel rune);
}