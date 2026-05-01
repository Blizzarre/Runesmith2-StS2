#region

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards;

public abstract class Runesmith2RecipeCard : Runesmith2Card
{
    protected override void AddExtraArgsToDescription(LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("IfFull", IsRuneSlotsFull());
    }

    protected override bool ShouldGlowGoldInternal => HasElements();

    protected override bool ShouldGlowRedInternal => IsRuneSlotsFull() && CanPlay();

    protected Runesmith2RecipeCard(int cost, CardType type, CardRarity rarity, TargetType target) : base(cost, type,
        rarity, target)
    {
        WithTags(RunesmithEnum.Recipe);
        WithTip(RunesmithHoverTip.Recipe);
        WithTip(RunesmithHoverTip.Craft);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Gains Elements instead
        if (IsPlayedWithoutElements)
        {
            await RunesmithPlayerCmd.GainElements(GetElementsCostWithModifiers().ClampZero(), Owner, cardPlay);
            return;
        }
        
        // Otherwise, use the actual card effects
        await RecipeOnPlayWrapper(choiceContext, cardPlay);
    }

    protected virtual Task RecipeOnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }
}