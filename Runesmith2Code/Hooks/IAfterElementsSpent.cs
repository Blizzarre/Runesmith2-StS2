#region

using MegaCrit.Sts2.Core.Entities.Players;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IAfterElementsSpent
{
    public Task AfterElementsSpent(Elements elements, Player spender);
}