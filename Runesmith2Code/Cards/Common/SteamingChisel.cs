#region

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Structs;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class SteamingChisel : Runesmith2Card
{
    public SteamingChisel() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithBlock(2, 1);
        WithDamage(3, 1);
        WithPower<WeakPower>(1, 1);
        WithVars(new IgnisVar(1), new AquaVar(1));
        WithTip(RunesmithHoverTip.Elements);
        WithTags(RunesmithTags.Chisel);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        await CommonActions.CardBlock(this, play);
        
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, play).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await CommonActions.Apply<WeakPower>(choiceContext, play.Target, this);

        await RunesmithPlayerCmd.GainElements(new Elements(this), Owner, play);
    }
}