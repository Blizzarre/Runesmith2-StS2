#region

using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Random;
using Runesmith2.Runesmith2Code.Commands;
using Runesmith2.Runesmith2Code.DynamicVars;
using Runesmith2.Runesmith2Code.Extensions;

#endregion

namespace Runesmith2.Runesmith2Code.Cards.Uncommon;

public class LendAHand : Runesmith2Card
{
    public LendAHand() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithVar(new EnhanceByVar(1));
        _portraitImage = GetPortraitImageIdx();
    }

    private int _portraitImage;

    protected override void AfterCloned()
    {
        base.AfterCloned();
        _portraitImage = GetPortraitImageIdx();
    }

    private static int GetPortraitImageIdx()
    {
        return Rng.Chaotic.NextInt(1, 4);
    }

    public override string CustomPortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_{_portraitImage}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
        }
    }

    public override string PortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_{_portraitImage}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
        }
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target?.Player);
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var targetPlayer = play.Target.Player;
        if (play.Target.IsPlayer)
        {
            var cards = PileType.Hand.GetPile(targetPlayer).Cards.Where(c => c.CanEnhance()).ToList();
            await RunesmithCardCmd.Enhance(choiceContext, Owner, cards, play,
                DynamicVars[EnhanceByVar.defaultName].IntValue);
            if (IsUpgraded)
                foreach (var card in cards)
                    RunesmithCardCmd.Stasis(card);
        }
    }
}