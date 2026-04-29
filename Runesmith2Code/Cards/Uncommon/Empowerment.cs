#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class Empowerment : Runesmith2Card
{
    public Empowerment() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(new TooltipSource(_ => HoverTipFactory.FromPower<VigorPower>()));
        WithTip(new TooltipSource(_ => HoverTipFactory.FromPower<BracePower>()));
        WithTip(new TooltipSource(_ => HoverTipFactory.FromPower<AmpPower>()));
        WithTip(RunesmithHoverTip.Elements);
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
    }

    public override Elements CanonicalElementsCost => new(1);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var elements = Owner.PlayerCombatState?.Elements() ?? new Elements();
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        if (elements.Ignis > 0) await CommonActions.ApplySelf<VigorPower>(choiceContext, this, elements.Ignis);
        if (elements.Terra > 0) await CommonActions.ApplySelf<BracePower>(choiceContext, this, elements.Terra);
        if (elements.Aqua > 0) await CommonActions.ApplySelf<AmpPower>(choiceContext, this, elements.Aqua);
    }
}