using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using Runesmith2.Runesmith2Code.Extensions;

namespace Runesmith2.Runesmith2Code.Enchantments;

public class Runesmith2Enchantment : CustomEnchantmentModel
{
    protected override string? CustomIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentImagePath();
            return ResourceLoader.Exists(path) ? path : null;
        }
    }
}