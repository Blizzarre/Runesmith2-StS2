#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class HotHammer : Runesmith2Card
{
    public HotHammer() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithVar(new CardsVar(2));
        WithVar(new EnhanceByVar(1));
        WithTags(RunesmithTags.Hammer);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        await RunesmithCardCmd.EnhanceRandomCards(choiceContext, Owner, PileType.Hand.GetPile(Owner).Cards,
            DynamicVars.Cards.IntValue, DynamicVars[EnhanceByVar.defaultName].IntValue,
            Owner.RunState.Rng.CombatCardSelection);
    }
}