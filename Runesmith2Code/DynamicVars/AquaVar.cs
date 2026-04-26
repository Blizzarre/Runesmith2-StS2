using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class AquaVar : DynamicVar
{
    public const string defaultName = "Aqua";

    public AquaVar(int aqua)
        : this(defaultName, aqua)
    {
    }

    public AquaVar(string name, int aqua)
        : base(name, aqua)
    {
    }
    
    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target,
        bool runGlobalHooks)
    {
        var modifiedValue = BaseValue;

        if (runGlobalHooks)
            modifiedValue = RunesmithHook.ModifyElementsGain(card.CombatState!, card.Owner, Elements.WithAqua(IntValue), ValueProp.Move, card, out _).Aqua;

        PreviewValue = modifiedValue;
    }
}