#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Models.Runes;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Morbus : Runesmith2RecipeCard
{
    public Morbus() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new ChargeVar(3).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Craft);
        WithRuneTip<MorbusRune>();
        WithTip(new TooltipSource(_ => HoverTipFactory.FromPower<WeakPower>()));
        WithTip(new TooltipSource(_ => HoverTipFactory.FromPower<VulnerablePower>()));
    }

    public override Elements CanonicalElementsCost => new(2, 0, 1);

    protected override async Task RecipeOnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await RuneCmd.Craft<MorbusRune>(choiceContext, Owner, play, this);
    }
}