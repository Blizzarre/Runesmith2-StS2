#region

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

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class MetalForming : Runesmith2Card
{
    public MetalForming() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCalculatedBlock(0, 3, (card, _) =>
            {
                if (card.CombatState == null || card.Owner.PlayerCombatState == null) return 0;
                return card.Owner.PlayerCombatState.GetElements().Terra +
                       RunesmithHook.ModifyElementsGain(card.CombatState, card.Owner,
                           Elements.WithTerra(card.DynamicVars[TerraVar.defaultName].IntValue),
                           ValueProp.Move, card, out var _).Terra;
            },
            ValueProp.Move, 0, 1);
        WithVars(new TerraVar(3));
        WithTip(RunesmithHoverTip.Elements);
        WithKeyword(CardKeyword.Exhaust);
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