using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Cards.Rare;

public class Tritium : Runesmith2Card
{
    public Tritium() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithEnergy(3);
        WithCards(3);
    }

    public override Elements CanonicalElementsCost => new(4);

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        UpgradeElementsCostBy(new Elements(-1));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
        await CommonActions.Draw(this, choiceContext);
    }
}