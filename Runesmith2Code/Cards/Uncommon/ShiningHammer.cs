#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class ShiningHammer : Runesmith2Card
{
    public ShiningHammer() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(13, 5);
        WithVar(new EnhanceByVar(1));
        WithTags(RunesmithTags.Hammer);
    }

    private static readonly HashSet<ModelId> ExtraHammerCards = [ModelDb.Card<HeirloomHammer>().Id];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);

        if (Owner.PlayerCombatState != null)
        {
            var currEnhance = this.GetEnhance();
            var cards = Owner.PlayerCombatState.AllPiles
                .Where(p => p.IsCombatPile && p.Type != PileType.Exhaust)
                .SelectMany(p => p.Cards)
                .Where(c => (c.Tags.Contains(RunesmithTags.Hammer) || ExtraHammerCards.Contains(c.Id)) &&
                            c.CanEnhance()).ToList();
            await RunesmithCardCmd.Enhance(choiceContext, Owner, cards, play,
                DynamicVars[EnhanceByVar.defaultName].IntValue);
            // Preserve self Enhance
            cards.FirstOrDefault(c => c == this)?.SetEnhanceAfterClear(this.GetEnhance() - currEnhance);
        }
    }
}