#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class HotHammer : Runesmith2Card
{
    public HotHammer() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        WithVar(new CardsVar(1).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Enhance);
        WithTags(RunesmithTag.Hammer);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        await RunesmithCardCmd.EnhanceRandomCards(choiceContext, Owner, PileType.Hand.GetPile(Owner).Cards,
            DynamicVars.Cards.IntValue, 1, Owner.RunState.Rng.CombatCardSelection);
    }
}