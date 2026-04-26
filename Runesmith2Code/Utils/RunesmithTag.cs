#region

using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

#endregion

namespace Runesmith2.Runesmith2Code.Utils;

public static class RunesmithTag
{
    [CustomEnum] public static CardTag Hammer;
    [CustomEnum] public static CardTag Chisel;
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