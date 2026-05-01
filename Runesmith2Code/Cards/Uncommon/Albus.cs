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

public class Albus : Runesmith2RecipeCard
{
    public Albus() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new ChargeVar(2).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Craft);
        WithRuneTip<AlbusRune>();
    }

    public override Elements CanonicalElementsCost => new(1, 1, 1);

    protected override async Task RecipeOnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await RuneCmd.Craft<AlbusRune>(choiceContext, Owner, play, this);
    }
}