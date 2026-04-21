using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class EnhanceByVar : DynamicVar
{
    public const string defaultName = "EnhanceBy";

    public EnhanceByVar(int amount)
        : this(defaultName, amount)
    {
    }

    public EnhanceByVar(string name, int amount)
        : base(name, amount)
    {
    }
}