using MegaCrit.Sts2.Core.Entities.Players;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IModifyElementsGain
{
    public Elements ModifyElementsGain(Player player, Elements amount);
}

public interface IAfterModifyingElementsGain
{
    public Task AfterModifyingElementsGain();
}