#region

using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Rewards;

#endregion

namespace Runesmith2.Runesmith2Code.Utils;

public static class RunesmithEnum
{
    [CustomEnum] public static CardTag Hammer;
    [CustomEnum] public static CardTag Chisel;
    [CustomEnum] public static CardTag Recipe;
}

public enum ElementType
{
    None,
    Ignis,
    Terra,
    Aqua,
    All
}

public enum ChargeDepletionType
{
    None,
    StartTurn,
    EndTurn
}