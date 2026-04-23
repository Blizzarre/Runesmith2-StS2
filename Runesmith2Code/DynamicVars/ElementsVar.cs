using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class ElementsVar : DynamicVar
{
    public const string defaultName = "Elements";

    public ElementsVar(int amount)
        : this(defaultName, amount)
    {
    }

    public ElementsVar(string name, int amount)
        : base(name, amount)
    {
    }
}