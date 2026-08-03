#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class Electromagnet : Runesmith2Card
{
    public Electromagnet() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(3, 3);
        WithTip(RunesmithHoverTip.Recipe);
        WithTip(RunesmithHoverTip.Improved);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var cardModel = (await CardSelectCmd.FromCombatPile(choiceContext,
            PileType.Discard.GetPile(Owner), Owner, prefs, c => c.IsImproved() || c.Tags.Contains(RunesmithTags.Recipe))).FirstOrDefault();
        if (cardModel != null) await CardPileCmd.Add(cardModel, PileType.Hand);
    }
}