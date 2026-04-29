#region

using Godot;
using Runesmith2.Runesmith2Code.Extensions;

#endregion

namespace Runesmith2.Runesmith2Code.Utils;

public static class RunesmithResource
{
    public static readonly Texture2D ElementsIcon =
        ResourceLoader.Load<Texture2D>("res://Runesmith2/images/charui/elements_all_icon.png");

    public const string NEnhanceTabPath = "res://Runesmith2/scenes/cards/enhance_tab.tscn";
    public const string NRuneManagerPath = "res://Runesmith2/scenes/runes/rune_manager.tscn";
    public const string NRunePath = "res://Runesmith2/scenes/runes/rune.tscn";
    public const string NElementsCounterPath = "res://Runesmith2/scenes/combat/energy_counters/elements_counter.tscn";
    public const string NElementsIconPath = "res://Runesmith2/scenes/cards/elements_icon.tscn";
    public const string NCreatureVisualsRunesmithPath = "res://Runesmith2/scenes/creature_visuals/runesmith.tscn";
    public const string NCharSelectBgRunesmithPath =
        "res://Runesmith2/scenes/screens/char_select/char_select_bg_runesmith.tscn";

    // These assets will be loaded with PreloadManager
    public static readonly IEnumerable<string> AssetPaths =
    [
        NEnhanceTabPath, NRuneManagerPath, NRunePath,
        NElementsCounterPath, NElementsIconPath, NCreatureVisualsRunesmithPath,
        NCharSelectBgRunesmithPath,
        "elements_ignis_icon.png".CharacterUiPath().ToRes(),
        "elements_terra_icon.png".CharacterUiPath().ToRes(),
        "elements_aqua_icon.png".CharacterUiPath().ToRes(),
        "elements_all_icon.png".CharacterUiPath().ToRes()
    ];
}