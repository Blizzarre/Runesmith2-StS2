using Godot;

namespace Runesmith2.Runesmith2Code.Utils;

// For storing loaded resource to base game scenes
public static class BaseResourceIndex
{
    public static readonly Font FontKreonRegularSpaceOne = ResourceLoader.Load<Font>("res://themes/kreon_regular_glyph_space_one.tres");
    public static readonly Font FontKreonBoldSpaceTwo = ResourceLoader.Load<Font>("res://themes/kreon_bold_glyph_space_two.tres");
}