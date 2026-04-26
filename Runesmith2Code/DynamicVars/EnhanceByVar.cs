using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class EnhanceByVar : DynamicVar
{
    public const string defaultName = "EnhanceBy";

    public EnhanceByVar(int amount)
        : this(defaultName, amount)
    {
    }

    public EnhanceByVar(string name, int amount)
        : base(name, amount)
    {
    }
    
    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target,
        bool runGlobalHooks)
    {
        var modifiedValue = BaseValue;

        if (runGlobalHooks)
            modifiedValue = RunesmithHook.ModifyEnhanceAmount(card.CombatState!, card.Owner, IntValue, ValueProp.Move, card, out _);

        PreviewValue = modifiedValue;
    }
}