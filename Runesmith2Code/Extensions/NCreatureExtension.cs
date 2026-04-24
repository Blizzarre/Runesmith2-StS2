using MegaCrit.Sts2.Core.Nodes.Combat;
using Runesmith2.Runesmith2Code.Field;
using Runesmith2.Runesmith2Code.Nodes.Runes;

namespace Runesmith2.Runesmith2Code.Extensions;

public static class NCreatureExtension
{
    public static NRuneManager? RuneManager(this NCreature creature)
    {
        return RunesmithNode.NRuneManager[creature];
    }
}