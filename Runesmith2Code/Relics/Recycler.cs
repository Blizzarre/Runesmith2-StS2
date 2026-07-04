#region

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Models;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Relics;

public class Recycler : Runesmith2Relic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ElementsVar(0) // For display
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        RunesmithHoverTipFactory.Static(RunesmithHoverTip.Break),
        RunesmithHoverTipFactory.Static(RunesmithHoverTip.Charge),
        RunesmithHoverTipFactory.CreateElementsHoverTip()
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;

        var runeQueue = Owner.PlayerCombatState?.GetRuneQueue();
        if (runeQueue == null) return;

        var index = 0;
        var elements = new Elements(0);
        RuneModel? currRune = null;
        while (index < runeQueue.Runes.Count)
        {
            var nextRune = runeQueue.Runes[index];
            if (nextRune.ChargeVal == 0 && nextRune != currRune)
            {
                Flash();

                currRune = nextRune;
                elements += nextRune.RecipeCard.CanonicalElementsCost;

                await RuneCmd.Break(choiceContext, Owner, currRune);
                await Cmd.CustomScaledWait(0.1f, 0.2f);
            }
            else
            {
                // increment index as rune wasn't broken
                index++;
            }
        }

        if (elements.Total > 0) await RunesmithPlayerCmd.GainElements(elements, Owner);
    }
}