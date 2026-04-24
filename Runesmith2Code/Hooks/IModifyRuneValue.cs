using MegaCrit.Sts2.Core.Entities.Players;

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IModifyRuneValue
{
    public decimal ModifyRuneValue(Player player, decimal value);
}