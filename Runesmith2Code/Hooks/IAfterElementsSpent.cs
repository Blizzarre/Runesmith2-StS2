using MegaCrit.Sts2.Core.Entities.Players;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IAfterElementsSpent
{
    public Task AfterElementsSpent(Elements elements, Player spender);
}