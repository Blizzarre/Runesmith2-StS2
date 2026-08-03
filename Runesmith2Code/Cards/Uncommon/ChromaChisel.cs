#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;
using Runesmith2.Runesmith2Code.Structs;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class ChromaChisel : Runesmith2Card
{
    public ChromaChisel() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(6);
        WithVars(new ElementsVar(1).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Elements);
        WithTags(RunesmithTags.Chisel);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await RunesmithPlayerCmd.GainElements(new Elements(this), Owner, play);
        
        var recipes = PileType.Draw.GetPile(Owner).Cards.Where(c => c.Tags.Contains(RunesmithTags.Recipe)).ToList();
        if (recipes.Count == 0) return;

        var card = Owner.RunState.Rng.CombatCardSelection.NextItem(recipes);
        if (card == null)
            return;
        
        await CardPileCmd.Add(card, PileType.Hand);
    }
}