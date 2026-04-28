#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class AugmentedBody : Runesmith2Card
{
    public AugmentedBody() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(RunesmithHoverTip.Enhance);
        WithTip(RunesmithHoverTip.Stasis);
        WithTip(new TooltipSource(_ => HoverTipFactory.FromKeyword(CardKeyword.Retain)));
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<AugmentedBodyPower>(choiceContext, this, 1);
        if (IsUpgraded)
        {
            var cards = PileType.Hand.GetPile(Owner).Cards;
            foreach (var card in cards) RunesmithCardCmd.Stasis(card);
        }
    }
}