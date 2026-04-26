using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Relics;

public class BrokenRuby : Runesmith2Relic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IgnisVar(2),
        new CardsVar(2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        RunesmithHoverTipFactory.CreateElementsHoverTip(),
        RunesmithHoverTipFactory.Static(RunesmithHoverTip.Enhance)
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
            await RunesmithPlayerCmd.GainElements(Elements.WithIgnis(DynamicVars[IgnisVar.defaultName].IntValue),
                Owner);
    }

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner && player.Creature.CombatState!.RoundNumber <= 1)
            await RunesmithCardCmd.EnhanceRandomCards(choiceContext, Owner, PileType.Hand.GetPile(Owner).Cards,
                DynamicVars.Cards.IntValue, 1, Owner.RunState.Rng.CombatCardSelection);
    }
}