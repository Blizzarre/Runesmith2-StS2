#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Scrapper : Runesmith2Card
{
    public Scrapper() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar("Amount", 1, 1);
        WithTip(RunesmithHoverTip.Break);
        WithTip(RunesmithHoverTip.Charge);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<ScrapperPower>(choiceContext, this, DynamicVars["Amount"].IntValue);
    }
}