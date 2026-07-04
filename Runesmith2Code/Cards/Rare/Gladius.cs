#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class Gladius : Runesmith2Card
{
    public Gladius() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithTags(RunesmithTags.Recipe);
        WithTip(RunesmithHoverTip.Recipe);
        WithDamage(40, 10);
    }

    protected override bool ShouldGlowGoldInternal => HasElements();

    public override TargetType TargetType => HasElements() ? TargetType.AllEnemies : TargetType.Self;

    public override Elements CanonicalElementsCost => new(4);

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

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play)
            .TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_giant_horizontal_slash")
            .Execute(choiceContext);
    }
}