#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class CatalyticConverter : Runesmith2Card
{
    public CatalyticConverter() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithVar(new ElementsVar(2).WithUpgrade(2));
        WithTip(RunesmithHoverTip.Elements);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await RunesmithPlayerCmd.GainElements(new Elements(this), Owner, play);
        await Cmd.CustomScaledWait(0.1f, 0.2f);
        await CommonActions.ApplySelf<CatalyticConverterPower>(choiceContext, this, 1);
    }
}