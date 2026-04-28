#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Prism : Runesmith2Card
{
    public Prism() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCalculatedBlock(0, 2,
            (card, _) => card.CombatState == null
                ? 0
                : (card.Owner.PlayerCombatState?.Elements() ?? new Elements()).Total,
            ValueProp.Move, 0, 1);
        WithTip(RunesmithHoverTip.Elements);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var elements = Owner.PlayerCombatState?.Elements() ?? new Elements(0);
        await CommonActions.CardBlock(this, DynamicVars.CalculatedBlock, play);
        if (elements.Total > 0) await RunesmithPlayerCmd.LoseElements(elements / 2, Owner);
    }
}