using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class BackupBattery : Runesmith2Card
{
    public BackupBattery() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithVar(new CardsVar(1));
        WithElementsVar(new ElementsVar(2).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Elements);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await RunesmithPlayerCmd.GainElements(new Elements(this), Owner, play);
        await CommonActions.Draw(this, choiceContext);
    }
}