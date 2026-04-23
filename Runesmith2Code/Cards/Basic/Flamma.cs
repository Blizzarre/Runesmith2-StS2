using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Models.Runes;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Cards.Basic;

public class Flamma : Runesmith2RecipeCard
{
    public Flamma() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithVars(new PotencyVar(4).WithUpgrade(1), new ChargeVar(2).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Recipe);
        WithTip(RunesmithHoverTip.Craft);
        WithTip(RunesmithHoverTip.Potency);
        WithTip(RunesmithHoverTip.Charge);
        WithRuneTip<FlammaRune>();
    }

    public override Elements CanonicalElementsCost => new(1, 0, 0);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await RuneCmd.Craft<FlammaRune>(choiceContext, Owner, cardPlay, DynamicVars[ChargeVar.defaultName].BaseValue,
            DynamicVars[PotencyVar.defaultName].BaseValue);
    }
}