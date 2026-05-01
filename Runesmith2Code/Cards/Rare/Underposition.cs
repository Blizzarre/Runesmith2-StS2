#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class Underposition : Runesmith2Card, IAfterRuneCrafted, IAfterRuneBroken
{
    public Underposition() : base(3, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithBlock(23, 8);
        WithTip(new TooltipSource(GetCardTip));
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }

    private async Task CheckAndTransformSelf()
    {
        var runeQueue = Owner.PlayerCombatState?.RuneQueue();
        if (runeQueue != null)
        {
            var count = runeQueue.Runes.Count;
            if (count % 2 == 0)
            {
                var targetCard = GetTransformedVersion();
                await CardCmd.Transform(this, targetCard);
            }
        }
    }

    private static IHoverTip GetCardTip(CardModel card)
    {
        return HoverTipFactory.FromCard(((Underposition)card).GetTransformedVersion());
    }

    private CardModel GetTransformedVersion()
    {
        CardModel targetCard;
        if (CombatState != null)
            targetCard = CombatState.CreateCard<Superposition>(Owner);
        else
            targetCard = (CardModel)ModelDb.Card<Superposition>().MutableClone();

        if (IsUpgraded) CardCmd.Upgrade(targetCard);
        if (Enchantment != null)
        {
            var enchantmentModel = (EnchantmentModel)Enchantment.MutableClone();
            CardCmd.Enchant(enchantmentModel, targetCard, enchantmentModel.Amount);
        }

        var enhance = this.GetEnhance();
        if (enhance > 0) targetCard.AddEnhance(enhance);

        var stasis = this.IsStasis();
        if (stasis) targetCard.SetStasis(stasis);

        return targetCard;
    }
    
    public async Task AfterRuneCrafted(PlayerChoiceContext choiceContext, Player player, RuneModel rune)
    {
        if (player == Owner && PileType.Hand.GetPile(player).Cards.Contains(this)) await CheckAndTransformSelf();
    }

    public async Task AfterRuneBroken(PlayerChoiceContext choiceContext, Player player, RuneModel rune)
    {
        if (player == Owner && PileType.Hand.GetPile(player).Cards.Contains(this)) await CheckAndTransformSelf();
    }
    
    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this) return;
        if (IsClone) return;
        await CheckAndTransformSelf();
    }
    
    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card == this && PileType.Hand.GetPile(Owner).Cards.Contains(this)) await CheckAndTransformSelf();
    }
}