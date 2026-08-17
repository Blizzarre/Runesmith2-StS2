#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class CalmBeforeStorm : Runesmith2Card
{
    public CalmBeforeStorm() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCards(1, 1);
        WithTip(RunesmithHoverTip.Enhance);
    }


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await CommonActions.ApplySelf<CalmBeforeStormPower>(choiceContext, this, DynamicVars.Cards.IntValue);
    }
}