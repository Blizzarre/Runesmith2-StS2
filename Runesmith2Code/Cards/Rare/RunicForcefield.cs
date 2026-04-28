#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Models;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class RunicForcefield : Runesmith2Card, IAfterRuneCrafted
{
    public RunicForcefield() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(9, 4);
        WithTip(RunesmithHoverTip.Craft);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
    }
    
    public async Task AfterRuneCrafted(PlayerChoiceContext choiceContext, Player player, RuneModel rune)
    {
        if (player != Owner) return;

        var handPile = PileType.Hand.GetPile(Owner);
        if (!handPile.Cards.Contains(this))
        {
            await CardPileCmd.Add(this, PileType.Hand);
        }
    }
}