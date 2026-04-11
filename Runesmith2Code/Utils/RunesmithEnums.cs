using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Runesmith2.Runesmith2Code.Utils;

public static class RunesmithEnums
{
    [CustomEnum] public static CardTag Hammer;
    [CustomEnum] public static CardTag Chisel;
    
    // todo patch card type extensions for Recipe type
    //  also patch other stuff needed for custom card type
    [CustomEnum] public static CardType Recipe;
}