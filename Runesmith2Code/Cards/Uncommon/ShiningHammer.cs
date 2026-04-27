#region

using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class ShiningHammer : Runesmith2Card
{

    public ShiningHammer() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(11, 1);
        WithVar(new EnhanceByVar(1).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Enhance);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);

        if (Owner.PlayerCombatState != null)
        {
            var cards = Owner.PlayerCombatState.AllPiles
                .Where(p => p.IsCombatPile && p.Type != PileType.Exhaust)
                .SelectMany(p => p.Cards)
                .Where(c => c != this && c.Tags.Contains(RunesmithTag.Hammer));
            await RunesmithCardCmd.Enhance(choiceContext, Owner, cards, play,
                DynamicVars[EnhanceByVar.defaultName].IntValue);
        }
    }
}