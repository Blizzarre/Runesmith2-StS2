#region

using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Hooks;

public interface IModifyElementsCost
{
    public bool TryModifyElementsCost(CardModel card, Elements originalCost, out Elements modifiedCost);
}