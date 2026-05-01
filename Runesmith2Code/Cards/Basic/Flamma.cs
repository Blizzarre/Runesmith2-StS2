#region

using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Cards.Ancient;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Models.Runes;
using Runesmith2.Runesmith2Code.Structs;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Basic;

public class Flamma : Runesmith2RecipeCard, ITranscendenceCard
{
    public Flamma() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithVars(new PotencyVar(4).WithUpgrade(1), new ChargeVar(2).WithUpgrade(1));
        WithTip(RunesmithHoverTip.Craft);
        WithRuneTip<FlammaRune>();
    }

    public override Elements CanonicalElementsCost => new(1, 0, 0);

    protected override async Task RecipeOnPlayWrapper(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await RuneCmd.Craft<FlammaRune>(choiceContext, Owner, play, this);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Fulgor>();
    }
}