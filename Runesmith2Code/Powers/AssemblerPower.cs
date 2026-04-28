using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Runesmith2.Runesmith2Code.Powers;

public class AssemblerPower : Runesmith2Power
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public CardModel? PickedCard { get; set; }

    protected override IEnumerable<IHoverTip> ExtraHoverTips {
        get
        {
            if (PickedCard == null) return [];
            return [HoverTipFactory.FromCard(PickedCard)];
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DisplayVar<AssemblerPower>("CardTitle", p => p.PickedCard?.Title ?? "")];

    public void PickCard(CardModel card)
    {
        PickedCard = card;
    }

    public override async Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;

        if (PickedCard != null)
        {
            Flash();
            await CardCmd.AutoPlay(choiceContext, PickedCard.CreateDupe(), null);
        }
    }
}