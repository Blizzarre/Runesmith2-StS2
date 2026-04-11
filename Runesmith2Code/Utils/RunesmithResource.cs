using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Runesmith2.Runesmith2Code.Utils;

public static class RunesmithResource
{
    public const string NEnhanceTabPath = "res://Runesmith2/scenes/cards/enhance_tab.tscn";
    public const string NRuneManagerPath = "res://Runesmith2/scenes/runes/rune_manager.tscn";
    public const string NRunePath = "res://Runesmith2/scenes/runes/rune.tscn";
    
    // TODO paths to 
    
    public static readonly IEnumerable<string> AssetPaths = [NEnhanceTabPath, NRuneManagerPath, NRunePath];
    
    // For base game scenes
    public static readonly PackedScene SelectionReticleScene = SceneHelper.Load("ui/selection_reticle");
}