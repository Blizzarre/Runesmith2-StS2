#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Common;

#endregion

namespace Runesmith2.Runesmith2Code.Models.Runes;

// Gain Block
public class SaxumRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 4;
    public override int ChargeVal { get; set; } = 3;

    public override bool UsePotency => true;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.EndTurn;

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Saxum>();

    public override async Task<bool> BeforeTurnEndRuneTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext);
        return true;
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        Trigger();
        PlayPassiveSfx();
        await GainBlock(choiceContext, PassiveVal);
        UseCharge();
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        await GainBlock(choiceContext, BreakVal);
    }

    private async Task GainBlock(PlayerChoiceContext _, decimal amount)
    {
        await CreatureCmd.GainBlock(Owner.Creature, amount, ValueProp.Unpowered, null);
    }
}