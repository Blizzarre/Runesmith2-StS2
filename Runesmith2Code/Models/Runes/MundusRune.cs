#region

using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Cards.Common;
using Runesmith2.Runesmith2Code.Nodes.Runes;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Models.Runes;

// Does nothing
public class MundusRune : RuneModel
{
    public override decimal PassiveVal { get; set; } = 4;
    public override int ChargeVal { get; set; } = 3;

    public override ChargeDepletionType ChargeDepletion => ChargeDepletionType.None;
    public override (decimal, decimal) BottomValue => (PassiveVal, PassiveVal);
    public override (Color, Color, Color) BottomBreakColor => NRune.DefaultFontColor;

    public override Runesmith2RecipeCard? RecipeCard => ModelDb.Get<Mundus>();
    
}