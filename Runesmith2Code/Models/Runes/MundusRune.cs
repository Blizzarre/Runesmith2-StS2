#region

using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Token;
using Runesmith2.Runesmith2Code.Cards.Uncommon;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Nodes.Runes;

#endregion

namespace Runesmith2.Runesmith2Code.Models.Runes;

// Does nothing
public class MundusRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 4;
    public override int ChargeVal { get; set; } = 3;

    public override bool UsePotency => true;

    public override bool CanPassive => false;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.None;
    public override (decimal, decimal) BottomValue => (PassiveVal, PassiveVal);

    public override Runesmith2RecipeCard RecipeCard => ModelDb.Get<Mundus>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var gemma = (Gemma)ModelDb.Card<Gemma>().ToMutable();
            gemma.SetEnhanceBy(ChargeVal);
            return [HoverTipFactory.FromCard(gemma)];
        }
    }

    public override async Task Break(PlayerChoiceContext choiceContext)
    {
        PlayPassiveSfx();
        await RunesmithCardCmd.GiveCard<Gemma>(Owner, PileType.Hand, skipAnimation: true, action: c => c.SetEnhanceBy(ChargeVal));
    }
}