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

public class ChargeVar : DynamicVar
{
    public const string defaultName = "Charge";

    public ChargeVar(int charge)
        : this(defaultName, charge)
    {
    }

    public ChargeVar(string name, int charge)
        : base(name, charge)
    {
        this.WithTooltip("RUNESMITH2-CHARGE");
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target,
        bool runGlobalHooks)
    {
        var modifiedValue = BaseValue;

        if (runGlobalHooks)
            modifiedValue = RunesmithHook.ModifyCharge(card.CombatState!, card.Owner, BaseValue, ValueProp.Move, card,
                null, out _);

        PreviewValue = modifiedValue;
    }
}