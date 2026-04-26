#region

using BaseLib.Abstracts;
using Godot;
using Runesmith2.Runesmith2Code.Extensions;

#endregion

namespace Runesmith2.Runesmith2Code.Character;

public class Runesmith2RelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Runesmith2.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}