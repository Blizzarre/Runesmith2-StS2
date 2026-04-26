using BaseLib.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Cards.Common;

public class ShiftingHammer : Runesmith2Card
{
    public ShiftingHammer() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
        WithVar(new CardsVar(1));
        WithTip(RunesmithHoverTip.Enhance);
        WithTags(RunesmithTag.Hammer);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        var attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(2).FromCard(this)
            .Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        var results = attackCommand.Results;
        var enhanceBy = results.Count(r => r.UnblockedDamage > 0);

        if (enhanceBy > 0)
        {
            var enhanceCard = (await CardSelectCmd.FromHand(
                choiceContext,
                Owner,
                new CardSelectorPrefs(RunesmithCardSelectorPrefs.EnhanceSelectionPrompt, DynamicVars.Cards.IntValue),
                card => card.CanEnhance(),
                this
            )).FirstOrDefault();
            if (enhanceCard != null) await RunesmithCardCmd.Enhance(choiceContext, Owner, enhanceCard, play, enhanceBy);
        }
    }
}