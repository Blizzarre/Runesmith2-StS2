#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Runesmith2.Runesmith2Code.Enchantments;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Ancient;

public class ArchitectsForge : Runesmith2Card
{
    public ArchitectsForge() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self)
    {
        WithVar("Amount", 2, 1);
        WithTips(c => HoverTipFactory.FromEnchantment<Forged>(c.DynamicVars["Amount"].IntValue));
        WithTip(RunesmithHoverTip.Enhance);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<ArchitectsForgePower>(choiceContext, this, 1);
    }
}