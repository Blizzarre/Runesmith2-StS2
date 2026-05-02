using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Potions;

public class RgbPotion : Runesmith2Potion
{
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new ElementsVar(3)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        RunesmithHoverTipFactory.CreateElementsHoverTip()
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        AssertValidForTargetedPotion(target);
        NCombatRoom.Instance?.PlaySplashVfx(target, new Color("0000ff"));
        NCombatRoom.Instance?.PlaySplashVfx(target, new Color("00ff00"));
        NCombatRoom.Instance?.PlaySplashVfx(target, new Color("ff0000"));
        NCombatRoom.Instance?.PlaySplashVfx(target, new Color("ffffff"));
        if (target.Player != null)
            await RunesmithPlayerCmd.GainElements(new Elements(DynamicVars[ElementsVar.defaultName].IntValue),
                target.Player);
    }
}