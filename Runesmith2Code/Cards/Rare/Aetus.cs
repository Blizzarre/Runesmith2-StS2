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

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class Aetus : Runesmith2RecipeCard
{
    public Aetus() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithVars(new ChargeVar(3).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Craft);
        WithRuneTip<AetusRune>();
        WithEnergyTip();
    }

    public override Elements CanonicalElementsCost => new(0, 0, 3);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await RuneCmd.Craft<AetusRune>(choiceContext, Owner, play, this);
    }
}