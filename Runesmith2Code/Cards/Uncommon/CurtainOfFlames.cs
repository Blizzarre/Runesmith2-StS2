#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Powers;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class CurtainOfFlames : Runesmith2Card
{
    public CurtainOfFlames() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<BracePower>(5, 2);
        WithVar(new IgnisVar(2).WithUpgrade(1));
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(Owner.Creature));
        await CommonActions.ApplySelf<BracePower>(choiceContext, this);
        await RunesmithPlayerCmd.GainElements(new Elements(this), Owner, play);
    }
}