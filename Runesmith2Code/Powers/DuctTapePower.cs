#region

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Hooks;

#endregion

namespace Runesmith2.Runesmith2Code.Powers;

public class DuctTapePower : Runesmith2Power, IAfterCardEnhanced
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    
    public Task AfterCardEnhanced(PlayerChoiceContext choiceContext, CardModel card, int enhanceAmount)
    {
        if (card.Owner != Owner.Player || card.IsStasis()) return Task.CompletedTask;
        Flash();
        RunesmithCardCmd.Stasis(card);
        return Task.CompletedTask;
    }
}