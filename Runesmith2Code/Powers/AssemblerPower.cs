#region

using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class AssemblerPower : Runesmith2Power
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private class Data
    {
        public CardModel? PickedCard;
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var pickedCard = GetInternalData<Data>().PickedCard;
            if (pickedCard == null) return [];
            return [HoverTipFactory.FromCard(pickedCard)];
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DisplayVar<AssemblerPower>("CardTitle", p => p.GetInternalData<Data>().PickedCard?.Title ?? "")];

    public void PickCard(CardModel card)
    {
        GetInternalData<Data>().PickedCard = card;
    }

    public override async Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;

        var internalData = GetInternalData<Data>();

        if (internalData.PickedCard != null)
        {
            Flash();
            await CardCmd.AutoPlay(choiceContext, internalData.PickedCard.CreateDupe(Owner.Player), null);
        }
    }
}