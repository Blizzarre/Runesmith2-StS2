using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Runesmith2.Runesmith2Code.Models;

public interface IRunesmithModel
{
    public virtual Task AfterCardEnhanced(PlayerChoiceContext choiceContext, CardModel card, int enhanceAmount)
    {
        return Task.CompletedTask;
    }
}