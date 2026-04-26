#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Hooks;

#endregion

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class PotencyVar : DynamicVar
{
    public const string defaultName = "Potency";

    public PotencyVar(int potency)
        : this(defaultName, potency)
    {
    }

    public PotencyVar(string name, int potency)
        : base(name, potency)
    {
        this.WithTooltip("RUNESMITH2-POTENCY");
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target,
        bool runGlobalHooks)
    {
        var modifiedValue = BaseValue;

        if (runGlobalHooks)
            modifiedValue = RunesmithHook.ModifyPotency(card.CombatState!, card.Owner, BaseValue, ValueProp.Move, card,
                null, out _);

        PreviewValue = modifiedValue;
    }
}