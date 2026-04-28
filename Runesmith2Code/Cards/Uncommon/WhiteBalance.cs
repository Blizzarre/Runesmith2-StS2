#region

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Combat;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class WhiteBalance : Runesmith2Card
{
    private const string CalculatedHitsKey = "CalculatedHits";

    public WhiteBalance() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithCalculatedDamage(0, 4, (card, _) =>
        {
            return CombatManager.Instance.History.Entries
                .OfType<ElementsModifiedEntry>()
                .Where(eme =>
                    eme.HappenedThisTurn(card.CombatState) && card.Owner == eme.Player && eme.Amount.Total > 0)
                .Select(eme => eme.Amount)
                .Aggregate(new Elements(0), (e1, e2) => e1 + e2)
                .Total;
        }, ValueProp.Move, 0, 1);
        WithTip(RunesmithHoverTip.Elements);
    }

    public override RuneBreakType RuneBreakType => RuneBreakType.Oldest;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);

        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        await RuneCmd.BreakOldest(choiceContext, Owner);
    }
}