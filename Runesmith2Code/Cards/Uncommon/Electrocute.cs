#region

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Electrocute : Runesmith2Card
{
    public Electrocute() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        WithCards(1);
        WithTip(RunesmithHoverTip.Stasis);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        VfxCmd.PlayOnCreature(play.Target, "vfx/vfx_attack_lightning");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .Targeting(play.Target)
            .Execute(choiceContext);

        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue);
        var cards = (await CardSelectCmd.FromCombatPile(choiceContext,
            PileType.Draw.GetPile(Owner), Owner, prefs)).ToList();
        foreach (var card in cards.Where(card => card.CanStasis()))
        {
            RunesmithCardCmd.Stasis(card);
        }
        await CardPileCmd.Add(cards, PileType.Draw, CardPilePosition.Top, null, true);
    }
}