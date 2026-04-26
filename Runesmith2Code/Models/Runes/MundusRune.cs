using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Basic;
using Runesmith2.Runesmith2Code.Cards.Common;
using Runesmith2.Runesmith2Code.Nodes.Runes;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Models.Runes;

public class MundusRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 4;
    public override int ChargeVal { get; set; } = 3;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.None;
    public override (decimal, decimal) BottomValue => (PassiveVal, PassiveVal);
    public override (Color, Color, Color) BottomBreakColor => NRune.DefaultFontColor;

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Mundus>();

    public override async Task BeforeTurnEndRuneTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext);
    }

    public override async Task Passive(PlayerChoiceContext choiceContext)
    {
        if (ChargeVal > 0)
        {
            Trigger();
            await GainBlock(choiceContext, PassiveVal);
            UseCharge();
        }
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