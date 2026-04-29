#region

using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

#endregion

namespace Runesmith2.Runesmith2Code.DynamicVars;

public class ChargeGainVar : DynamicVar
{
    public const string defaultName = "ChargeGain";

    public ChargeGainVar(int charge)
        : this(defaultName, charge)
    {
    }

    public ChargeGainVar(string name, int charge)
        : base(name, charge)
    {
        this.WithTooltip("RUNESMITH2-CHARGE");
    }
}