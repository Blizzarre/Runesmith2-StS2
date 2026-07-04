#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Prisma : Runesmith2Card
{
    public Prisma() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTags(RunesmithTags.Recipe);
        WithTip(RunesmithHoverTip.Recipe);
        WithDamage(8, 3);
        WithBlock(6, 2);
        WithCards(2, 1);
    }
    
    protected override bool ShouldGlowGoldInternal => HasElements();

    public override Elements CanonicalElementsCost => new(1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsPlayedWithoutElements)
        {
            await RunesmithPlayerCmd.GainElements(GetElementsCostWithModifiers().ClampZero(), Owner, cardPlay);
            return;
        }

        await RecipeOnPlayWrapper(choiceContext, cardPlay);
    }
    
    private async Task RecipeOnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        if (CombatState == null) return;

        ArgumentNullException.ThrowIfNull(play.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await CommonActions.CardBlock(this, play);

        await CommonActions.Draw(this, choiceContext);
    }
}