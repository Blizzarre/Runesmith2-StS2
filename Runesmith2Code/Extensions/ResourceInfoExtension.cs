using Runesmith2.Runesmith2Code.Structs;

namespace Runesmith2.Runesmith2Code.Extensions;

public static class ResourceInfoExtension
{
    public class RunesmithResourceInfo
    {
        public required Elements ElementsSpent { get; init; }
        public required Elements ElementsValue { get; init; }
    }
}