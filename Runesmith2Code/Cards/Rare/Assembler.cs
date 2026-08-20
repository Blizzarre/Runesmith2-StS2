#region

using BaseLib.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class Assembler : Runesmith2Card
{
    public Assembler() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithTip(RunesmithHoverTip.Craft);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);

        var cardModel = (await MultiPileCardSelect.Select(choiceContext, Owner, prefs,
            c => c.Tags.Contains(RunesmithTags.Recipe), PileType.Hand, PileType.Discard)).FirstOrDefault();

        if (cardModel != null)
        {
            var power = (AssemblerPower)ModelDb.Power<AssemblerPower>().MutableClone();
            power.PickCard(cardModel);
            await PowerCmd.Apply(choiceContext, power, Owner.Creature, 1, Owner.Creature, this);
            await CardCmd.Exhaust(choiceContext, cardModel);
        }
    }
}