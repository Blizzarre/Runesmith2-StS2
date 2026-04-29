#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Hooks;

#endregion

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class EnhanceByVar : DynamicVar
{
    public const string defaultName = "EnhanceBy";

    public EnhanceByVar(int amount, bool tip = true)
        : this(defaultName, amount, tip)
    {
    }

    public EnhanceByVar(string name, int amount, bool tip)
        : base(name, amount)
    {
        BaseValue = amount;
        if (tip) this.WithTooltip("RUNESMITH2-ENHANCE");
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target,
        bool runGlobalHooks)
    {
        var modifiedValue = BaseValue;

        if (runGlobalHooks)
            modifiedValue =
                RunesmithHook.ModifyEnhanceAmount(card.CombatState!, card.Owner, IntValue, card, out _);

        PreviewValue = modifiedValue;
    }
}