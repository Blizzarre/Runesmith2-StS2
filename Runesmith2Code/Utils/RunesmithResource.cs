using Godot;

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
    public const string ElementsCostPath = "res://Runesmith2/images/cardui/elements_cost.png";

    // These assets will be loaded with PreloadManager
    public static readonly IEnumerable<string> AssetPaths =
    [
        NEnhanceTabPath, NRuneManagerPath, NRunePath, NElementsCounterPath, ElementsCostPath,
        "res://Runesmith2/images/charui/elements_ignis_icon.png",
        "res://Runesmith2/images/charui/elements_terra_icon.png",
        "res://Runesmith2/images/charui/elements_aqua_icon.png",
        "res://Runesmith2/images/charui/elements_all_icon.png"
    ];
}