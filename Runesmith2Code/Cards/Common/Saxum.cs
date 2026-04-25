using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Models.Runes;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class Saxum : Runesmith2RecipeCard
{
    public Saxum() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithVars(new PotencyVar(4).WithUpgrade(2), new ChargeVar(3));
        WithTip(RunesmithHoverTip.Recipe);
        WithTip(RunesmithHoverTip.Craft);
        WithTip(RunesmithHoverTip.Potency);
        WithTip(RunesmithHoverTip.Charge);
        WithRuneTip<SaxumRune>();
    }

    public override Elements CanonicalElementsCost => new(0, 2, 0);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await RuneCmd.Craft<SaxumRune>(choiceContext, Owner, cardPlay, this);
    }
}