using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class PotencyVar : DynamicVar
{
    public const string defaultName = "Potency";

    public PotencyVar(int potency)
        : this(defaultName, potency)
    {
    }

    public PotencyVar(string name, int potency)
        : base(name, potency)
    {
    }
}