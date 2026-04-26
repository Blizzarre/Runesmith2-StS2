#region

using Godot;

#endregion

namespace Runesmith2.Runesmith2Code.Utils;

// For storing loaded resource to base game scenes
public static class BaseResourceIndex
{
    public static Font FontKreonRegularSpaceOne =>
        ResourceLoader.Load<Font>("res://themes/kreon_regular_glyph_space_one.tres");

    public static Font FontKreonBoldSpaceTwo =>
        ResourceLoader.Load<Font>("res://themes/kreon_bold_glyph_space_two.tres");

    public static Font FontKreonBoldShared => ResourceLoader.Load<Font>("res://themes/kreon_bold_shared.tres");

    public static AtlasTexture CardUnplayableIcon =>
        ResourceLoader.Load<AtlasTexture>("res://images/atlases/ui_atlas.sprites/card/card_unplayable_icon.tres");
}