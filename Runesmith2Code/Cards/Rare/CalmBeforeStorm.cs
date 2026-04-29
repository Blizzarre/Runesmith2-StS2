#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class CalmBeforeStorm : Runesmith2Card
{
    public CalmBeforeStorm() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPower<BracePower>(5);
        WithCards(2);
        WithEnergy(1, 1);
        WithEnergyTip();
    }

    public override RuneBreakType RuneBreakType => RuneBreakType.Oldest;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        await CommonActions.ApplySelf<BracePower>(choiceContext, this);
        await CommonActions.ApplySelf<EnergyNextTurnPower>(choiceContext, this, DynamicVars.Energy.IntValue);
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(choiceContext, this, DynamicVars.Cards.IntValue);
    }
}