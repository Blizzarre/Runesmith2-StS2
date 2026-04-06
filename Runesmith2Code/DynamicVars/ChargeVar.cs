using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class ChargeVar : DynamicVar
{
    public const string defaultName = "Charge";

    public ChargeVar(int charge)
        : this(defaultName, charge)
    {
    }

    public ChargeVar(string name, int charge)
        : base(name, charge)
    {
    }
}