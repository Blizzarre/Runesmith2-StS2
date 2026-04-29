#region

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Electrocute : Runesmith2Card
{
    public Electrocute() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 4);
        WithCards(1);
        WithTip(RunesmithHoverTip.Stasis);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        VfxCmd.PlayOnCreature(play.Target, "vfx/vfx_attack_lightning");
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .Targeting(play.Target)
            .Execute(choiceContext);

        // RuneCmd.ChargeNewest(choiceContext, Owner, DynamicVars[ChargeGainVar.defaultName].IntValue);

        var prefs = new CardSelectorPrefs(RunesmithCardSelectorPrefs.StasisSelectionPrompt, DynamicVars.Cards.IntValue);
        var pile = PileType.Draw.GetPile(Owner);
        var cards = await CardSelectCmd.FromSimpleGrid(choiceContext,
            pile.Cards.Where(c => c.CanStasis()).ToList(), Owner, prefs);
        foreach (var card in cards) RunesmithCardCmd.Stasis(card);
    }
}