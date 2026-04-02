using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Runesmith2.Runesmith2Code.Character;
using Runesmith2.Runesmith2Code.Extensions;
using Godot;

namespace Runesmith2.Runesmith2Code.Relics;

[Pool(typeof(Runesmith2RelicPool))]
public abstract class Runesmith2Relic : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
        }
    }
}