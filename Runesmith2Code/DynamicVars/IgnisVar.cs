using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class IgnisVar : DynamicVar
{
    public const string defaultName = "Ignis";

    public IgnisVar(int ignis)
        : this(defaultName, ignis)
    {
    }

    public IgnisVar(string name, int ignis)
        : base(name, ignis)
    {
    }
}