#region

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;

#endregion

namespace Runesmith2.Runesmith2Code.Relics;

public class Nanobots : Runesmith2Relic
{

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("ExtraBlock", 1),
        new("ExtraDamage", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        RunesmithHoverTipFactory.Static(RunesmithHoverTip.Enhance),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
    
    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource == null || cardSource.Owner != Owner || Owner.Creature != target || !props.IsCardOrMonsterMove() || !cardSource.IsEnhanced())
        {
            return 0m;
        }
        
        return DynamicVars["ExtraBlock"].IntValue;
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource == null || cardSource.Owner != Owner || Owner.Creature != dealer || !props.IsPoweredAttack() || !cardSource.IsEnhanced())
        {
            return 0m;
        }

        return DynamicVars["ExtraDamage"].IntValue;
    }
}