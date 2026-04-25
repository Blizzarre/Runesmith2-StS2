using MegaCrit.Sts2.Core.Localization;

namespace Runesmith2.Runesmith2Code.CardSelection;

public static class RunesmithCardSelectorPrefs
{
    public static LocString EnhanceSelectionPrompt => new("card_selection", "RUNESMITH2-TO_ENHANCE");
    public static LocString StasisSelectionPrompt => new("card_selection", "RUNESMITH2-TO_STASIS");
}