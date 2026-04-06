using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class AquaVar : DynamicVar
{
    public const string defaultName = "Aqua";

    public AquaVar(int aqua)
        : this(defaultName, aqua)
    {
    }

    public AquaVar(string name, int aqua)
        : base(name, aqua)
    {
    }
}