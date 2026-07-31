#region

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class ShiftingHammer : Runesmith2Card
{
    public ShiftingHammer() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
        WithVar(new EnhanceByVar(2));
        WithVar(new CardsVar(1));
        WithTags(RunesmithTags.Hammer);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(2).FromCard(this, play)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        var cards = await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            new CardSelectorPrefs(RunesmithCardSelectorPrefs.EnhanceSelectionPrompt, DynamicVars.Cards.IntValue),
            card => card.CanEnhance(),
            this
        );
        await RunesmithCardCmd.Enhance(choiceContext, Owner, cards, play,
            DynamicVars[EnhanceByVar.defaultName].IntValue);
    }
}