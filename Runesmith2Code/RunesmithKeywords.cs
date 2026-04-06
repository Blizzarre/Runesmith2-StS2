using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Runesmith2.Runesmith2Code;

public static class RunesmithKeywords
{
    // todo put this somewhere else, maybe?
    [CustomEnum] public static CardTag Hammer;
    [CustomEnum] public static CardTag Chisel;
    
    // todo patch card type extensions for Recipe type
    //  also patch other stuff needed for custom card type
    [CustomEnum] public static CardType Recipe;
}