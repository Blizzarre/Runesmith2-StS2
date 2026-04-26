#region

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class IgnisVar : DynamicVar
{
    public const string defaultName = "Ignis";

    public IgnisVar(int ignis)
        : this(defaultName, ignis)
    {
    }

    public IgnisVar(string name, int ignis)
        : base(name, ignis)
    {
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target,
        bool runGlobalHooks)
    {
        var modifiedValue = BaseValue;

        if (runGlobalHooks)
            modifiedValue = RunesmithHook.ModifyElementsGain(card.CombatState!, card.Owner,
                Elements.WithIgnis(IntValue), ValueProp.Move, card, out _).Ignis;

        PreviewValue = modifiedValue;
    }
}