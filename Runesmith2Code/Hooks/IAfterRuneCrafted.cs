using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Models;

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IAfterRuneCrafted
{
    public Task AfterRuneCrafted(PlayerChoiceContext choiceContext, Player player, RuneModel rune);
}