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

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class GelidCanister : Runesmith2Card
{
    public GelidCanister() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithPower<IceColdPower>(4, 1);
        WithVars(new AquaVar(2).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Elements);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (CombatState != null)
            foreach (var hittableEnemy in CombatState.HittableEnemies)
                await CommonActions.Apply<IceColdPower>(choiceContext, hittableEnemy, this);

        await RunesmithPlayerCmd.GainElements(new Elements(this), Owner, play);
    }
}