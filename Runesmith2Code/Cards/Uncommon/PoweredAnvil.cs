#region

using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Runesmith2.Runesmith2Code.CardSelection;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Powers;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class PoweredAnvil : Runesmith2Card
{
    public PoweredAnvil() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar(new EnhanceByVar(0).WithUpgrade(2));
        WithTip(RunesmithHoverTip.Enhance);
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (IsUpgraded)
        {
            var card = (await CardSelectCmd.FromHand(
                choiceContext,
                Owner,
                new CardSelectorPrefs(RunesmithCardSelectorPrefs.EnhanceSelectionPrompt, 1),
                card => card.CanEnhance(),
                this
            )).FirstOrDefault();
            if (card != null)
                await RunesmithCardCmd.Enhance(choiceContext, Owner, card, play,
                    DynamicVars[EnhanceByVar.defaultName].IntValue);
        }

        await CommonActions.ApplySelf<PoweredAnvilPower>(choiceContext,this,  1);
    }
}