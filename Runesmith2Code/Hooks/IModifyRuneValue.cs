#region

using MegaCrit.Sts2.Core.Entities.Players;

#endregion

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IModifyRuneValue
{
    public decimal ModifyRuneValue(Player player, decimal value);
}