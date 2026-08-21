#region

using BaseLib.Abstracts;
using BaseLib.Patches.UI;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Runesmith2.Runesmith2Code.Cards.Basic;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Relics;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Character;

public class Runesmith2 : PlaceholderCharacterModel
{
    public const string CharacterId = "Runesmith2";

    public static readonly Color Color = new("99785f");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 75;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeRunesmith>(),
        ModelDb.Card<StrikeRunesmith>(),
        ModelDb.Card<StrikeRunesmith>(),
        ModelDb.Card<StrikeRunesmith>(),
        ModelDb.Card<DefendRunesmith>(),
        ModelDb.Card<DefendRunesmith>(),
        ModelDb.Card<DefendRunesmith>(),
        ModelDb.Card<DefendRunesmith>(),
        ModelDb.Card<Fortify>(),
        ModelDb.Card<Flamma>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BrokenRuby>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<Runesmith2CardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<Runesmith2RelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<Runesmith2PotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override string CustomIconTexturePath => "character_icon_runesmith.png".CharacterUiPath();
    protected override string IconPath => "runesmith_icon".ScenePath("top_panel");
    public override string CustomIconOutlineTexturePath => "character_icon_runesmith_outline.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_runesmith.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_runesmith_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_runesmith.png".CharacterUiPath();

    public override string CustomCharacterSelectTransitionPath => "runesmith_transition_mat.tres".CharacterUiPath();
    public override string CustomArmPointingTexturePath => "multiplayer_hand_runesmith_point.png".CharacterUiPath();
    public override string CustomArmRockTexturePath => "multiplayer_hand_runesmith_rock.png".CharacterUiPath();
    public override string CustomArmPaperTexturePath => "multiplayer_hand_runesmith_paper.png".CharacterUiPath();
    public override string CustomArmScissorsTexturePath => "multiplayer_hand_runesmith_scissors.png".CharacterUiPath();

    public override string CustomEnergyCounterPath =>
        "res://Runesmith2/scenes/combat/energy_counters/runesmith_energy_counter.tscn";

    public override Color EnergyLabelOutlineColor => new("5b4a31");

    public override Color DialogueColor => new("59422d");

    public override VfxColor SpeechBubbleColor => VfxColor.Gold;

    public override Color MapDrawingColor => new("69543f");

    public override Color RemoteTargetingLineColor => new("c4784fff");

    public override Color RemoteTargetingLineOutline => new("6b492bff");

    public override string CustomTrailPath => "card_trail_runesmith".ScenePath("vfx");

    public override NCreatureVisuals CreateCustomVisuals()
    {
        return NodeFactory<NCreatureVisuals>.CreateFromScene(RunesmithResource.NCreatureVisualsRunesmithPath);
    }

    public override string CustomMerchantAnimPath => "runesmith_merchant".ScenePath("merchant");

    public override string CustomRestSiteAnimPath => "runesmith_rest_site".ScenePath("rest_site");

    public override string CustomCharacterSelectBg => RunesmithResource.NCharSelectBgRunesmithPath;

    public override string CharacterTransitionSfx => "res://Runesmith2/audio/runesmith_character_transition.ogg";
    public override string CharacterSelectSfx => "res://Runesmith2/audio/runesmith_character_select.ogg";

    public override RelicIconData? CustomYummyCookie => new(
        "yummy_cookie_runesmith.png".BigRelicImagePath(),
        "yummy_cookie_runesmith.png".RelicImagePath(),
        "yummy_cookie_runesmith_outline.png".RelicImagePath()
        );
}