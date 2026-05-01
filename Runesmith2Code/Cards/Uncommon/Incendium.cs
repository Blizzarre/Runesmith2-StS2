#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Models.Runes;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Incendium : Runesmith2RecipeCard
{
    public Incendium() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new PotencyVar(4).WithUpgrade(2), new ChargeVar(3));
        WithTip(RunesmithHoverTip.Craft);
        WithRuneTip<IncendiumRune>();
    }

    public override Elements CanonicalElementsCost => new(3, 0, 0);

    protected override async Task RecipeOnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await RuneCmd.Craft<IncendiumRune>(choiceContext, Owner, play, this);
    }
}