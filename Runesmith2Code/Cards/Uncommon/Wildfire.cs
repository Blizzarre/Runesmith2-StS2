#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Wildfire : Runesmith2Card
{
    public Wildfire() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar(new PowerVar<WildfirePower>(1));
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithTip(RunesmithHoverTip.Elements);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<WildfirePower>(choiceContext, this);
    }
}