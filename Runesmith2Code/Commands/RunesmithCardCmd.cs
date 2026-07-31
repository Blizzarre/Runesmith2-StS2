#region

using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.Nodes.Vfx;
using Runesmith2.Runesmith2Code.Utils;
using MethodInfo = System.Reflection.MethodInfo;

#endregion

namespace Runesmith2.Runesmith2Code.Commands;

public static class RunesmithCardCmd
{
    public static async Task Enhance(PlayerChoiceContext choiceContext, Player player,
        CardModel targetCard,
        CardPlay? cardPlay, int enhanceAmount, bool skipVisuals = false)
    {
        await Enhance(choiceContext, player, [targetCard], cardPlay, enhanceAmount, skipVisuals);
    }

    public static async Task Enhance(PlayerChoiceContext choiceContext, Player player,
        IEnumerable<CardModel> targetCards,
        CardPlay? cardPlay, int enhanceAmount, bool skipVisuals = false)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            var combatState = player.Creature.CombatState;
            var cardList = targetCards.ToList();
            if (combatState == null || cardList.Count == 0) return;
            // NOTE Consider adding history for cards enhanced.
            var modifiedEnhance =
                RunesmithHook.ModifyEnhanceAmount(combatState, player, enhanceAmount, cardPlay?.Card,
                    out var modifiers);
            await RunesmithHook.AfterModifyingEnhanceAmount(modifiedEnhance, cardPlay?.Card, cardPlay,
                modifiers);

            foreach (var targetCard in cardList)
            {
                if (!targetCard.CanEnhance())
                {
                    Runesmith2Mod.Logger.Warn($"Cannot enhance {targetCard.Id}.");
                    continue;
                }

                targetCard.AddEnhance(modifiedEnhance);
                if (!skipVisuals && RunesmithConfig.EnableEnhanceVfx)
                {
                    var cardNode = NCard.FindOnTable(targetCard);
                    NCardEnhanceVfx? vfx = null;
                    if (cardNode != null) vfx = NCardEnhanceVfx.Create(cardNode);
                    if (vfx != null) _ = TaskHelper.RunSafely(vfx.PlayAnimation());
                }
                await RunesmithHook.AfterCardEnhanced(combatState, choiceContext, player, targetCard, cardPlay, modifiedEnhance);
            }
        }
    }

    // Add Enhance without triggering hooks
    public static void AddEnhance(IEnumerable<CardModel> targetCards, int enhanceAmount, bool skipVisuals = false)
    {
        if (!CombatManager.Instance.IsOverOrEnding)
        {
            foreach (var targetCard in targetCards)
            {
                if (!targetCard.CanEnhance())
                {
                    Runesmith2Mod.Logger.Warn($"Cannot enhance {targetCard.Id}.");
                    continue;
                }

                targetCard.AddEnhance(enhanceAmount);
                
                if (skipVisuals) continue;
                
                var cardNode = NCard.FindOnTable(targetCard);
                NCardEnhanceVfx? vfx = null;
                if (cardNode != null) vfx = NCardEnhanceVfx.Create(cardNode);
                if (vfx != null) _ = TaskHelper.RunSafely(vfx.PlayAnimation());
            }
        }
    }

    public static async Task EnhanceRandomCards(PlayerChoiceContext choiceContext, Player player,
        IEnumerable<CardModel> cards, int cardCount, int enhanceBy, Rng rng, bool skipVisuals = false)
    {
        var randomCards = new List<CardModel>(cards.Where(c => c.CanEnhance())).StableShuffle(rng);
        await Enhance(choiceContext, player, randomCards.Take(cardCount), null, enhanceBy);
    }

    public static void Stasis(CardModel targetCard)
    {
        if (CombatManager.Instance.IsOverOrEnding) return;
        if (!targetCard.CanEnhance()) throw new InvalidOperationException($"Cannot stasis {targetCard.Id}.");

        if (targetCard.IsStasis()) return;

        targetCard.SetStasis(true);

        if (!RunesmithConfig.EnableStasisVfx) return;
        
        var cardNode = NCard.FindOnTable(targetCard);
        if (cardNode == null) return;

        var vfx = NCardStasisVfx.Create(cardNode);
        if (vfx == null) return;

        TaskHelper.RunSafely(vfx.PlayAnimation());
    }

    // Code taken from https://github.com/lamali292/Downfall/blob/develop-2/DownfallCode/Commands/DownfallCardCmd.cs
    public static async Task<T> GiveCard<T>(Player player,
        PileType pileType,
        CardPilePosition position = CardPilePosition.Bottom,
        bool upgraded = false,
        float animationTime = 0.6f,
        CardPreviewStyle animationStyle = CardPreviewStyle.HorizontalLayout,
        bool skipAnimation = false,
        Action<T>? action = null) where T : CardModel
    {
        var card = (T)player.Creature.CombatState!.CreateCard(ModelDb.Card<T>(), player);
        if (upgraded) card.UpgradeInternal();
        action?.Invoke(card);
        var result = await CardPileCmd.AddGeneratedCardToCombat(card, pileType, player, position);
        if (result.success && !skipAnimation && pileType != PileType.Hand)
            CardCmd.PreviewCardPileAdd(result, animationTime, animationStyle);
        return (T)result.cardAdded;
    }

    private static readonly MethodInfo CreateCardNodeMethod =
        AccessTools.Method(typeof(CardPileCmd), "CreateCardNodeAndUpdateVisuals", [typeof(CardModel), typeof(PileType), typeof(bool)]);
}