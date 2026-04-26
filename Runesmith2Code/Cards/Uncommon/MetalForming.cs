using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class MetalForming : Runesmith2Card
{
    
    // TODO This card is super jank. Find a better way to calculate preview and gain block.
    
    public MetalForming() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        var terra = new TerraVar(1);
        // TODO check if the calc account for additional terra gain from Synthetic Blood
        WithCalculatedBlock(0, 2, (card, _) =>
            {
                if (card.CombatState == null) return 0;
                return card.Owner.PlayerCombatState?.Elements().Terra +
                    RunesmithHook.ModifyElementsGain(card.CombatState, card.Owner, Elements.WithTerra(terra.IntValue),
                        ValueProp.Move, card, out var _).Terra ?? 0;
            },
            ValueProp.Move, 0, 1);
        WithVars(terra);
        WithTip(RunesmithHoverTip.Elements);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var blockToGain = new BlockVar(DynamicVars.CalculatedBlock.Calculate(Owner.Creature), ValueProp.Move);
        await RunesmithPlayerCmd.GainElements(new Elements(this), Owner);
        await CommonActions.CardBlock(this, blockToGain, play);
    }
}