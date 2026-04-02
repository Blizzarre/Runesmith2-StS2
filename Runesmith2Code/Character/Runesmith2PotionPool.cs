using BaseLib.Abstracts;
using Runesmith2.Runesmith2Code.Extensions;
using Godot;

namespace Runesmith2.Runesmith2Code.Character;

public class Runesmith2PotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Runesmith2.Color;
    

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}