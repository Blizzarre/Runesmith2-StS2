#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class Grindstone : Runesmith2Card
{
    public Grindstone() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6);
        WithVar("Amount", 1, 1);
        WithTip(RunesmithHoverTip.Enhance);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);

        var power = (GrindstonePower)ModelDb.Power<GrindstonePower>().MutableClone();
        power.SetOwnerCard(this);
        await PowerCmd.Apply(choiceContext, power, Owner.Creature, DynamicVars["Amount"].BaseValue, Owner.Creature,
            this);
    }
}