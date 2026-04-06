using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class TerraVar : DynamicVar
{
    public const string defaultName = "Terra";

    public TerraVar(int terra)
        : this(defaultName, terra)
    {
    }

    public TerraVar(string name, int terra)
        : base(name, terra)
    {
    }
}