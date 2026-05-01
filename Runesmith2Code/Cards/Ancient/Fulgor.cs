#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Models.Runes;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Ancient;

public class Fulgor : Runesmith2RecipeCard
{
    public Fulgor() : base(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
        WithVars(new PotencyVar(5).WithUpgrade(2), new ChargeVar(4).WithUpgrade(1));
        WithRuneTip<FulgorRune>();
    }

    public override Elements CanonicalElementsCost => new(1, 0, 0);

    protected override async Task RecipeOnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await RuneCmd.Craft<FulgorRune>(choiceContext, Owner, play, this);
    }
}