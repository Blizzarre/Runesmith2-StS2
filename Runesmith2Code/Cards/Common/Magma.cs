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

public class Magma : Runesmith2RecipeCard
{
    public Magma() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithVars(new PotencyVar(4).WithUpgrade(1), new ChargeVar(2).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Recipe);
        WithTip(RunesmithHoverTip.Craft);
        WithRuneTip<MagmaRune>();
    }

    public override Elements CanonicalElementsCost => new(2, 1, 0);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await RuneCmd.Craft<MagmaRune>(choiceContext, Owner, play, this);
    }
}