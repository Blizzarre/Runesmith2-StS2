#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class DuctTape : Runesmith2Card
{
    public DuctTape() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithTip(RunesmithHoverTip.Enhance);
        WithTip(RunesmithHoverTip.Stasis);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<DuctTapePower>(choiceContext, this, 1);
    }
}