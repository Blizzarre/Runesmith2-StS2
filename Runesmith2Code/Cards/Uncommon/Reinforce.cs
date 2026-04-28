#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Reinforce : Runesmith2Card
{
    public Reinforce() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(1);
        WithVar(new EnhanceByVar(1, false));
        WithTips(c =>
        {
            if (c.IsUpgraded) return [RunesmithHoverTipFactory.Static(RunesmithHoverTip.Enhance)];

            return [];
        });
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.Draw(this, choiceContext);
        var cards = PileType.Hand.GetPile(Owner).Cards;

        foreach (var card in cards)
            if (card.IsUpgradable)
                CardCmd.Upgrade(card);

        if (IsUpgraded)
            await RunesmithCardCmd.Enhance(choiceContext, Owner, cards.Where(c => c.CanEnhance()), play,
                DynamicVars[EnhanceByVar.defaultName].IntValue);
    }
}