#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;


public class Electromagnet : Runesmith2Card
{
    public Electromagnet() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(2, 4);
        WithTip(RunesmithHoverTip.Enhance);
        WithTip(RunesmithHoverTip.Stasis);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var pile = PileType.Discard.GetPile(Owner);
        var cardModel = (await CardSelectCmd.FromSimpleGrid(choiceContext,
            pile.Cards.Where(c => c.IsEnhanced() || c.IsStasis()).ToList(), Owner, prefs)).FirstOrDefault();
        if (cardModel != null)
        {
            await CardPileCmd.Add(cardModel, PileType.Hand);
        }
    }
}