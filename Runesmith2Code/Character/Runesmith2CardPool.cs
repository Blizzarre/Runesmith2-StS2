#region

using BaseLib.Abstracts;
using Godot;
using Runesmith2.Runesmith2Code.Extensions;

#endregion

namespace Runesmith2.Runesmith2Code.Character;

public class Runesmith2CardPool : CustomCardPoolModel
{
    public override string Title => Runesmith2.CharacterId; //This is not a display name.

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();


    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 0.102f; //Hue; changes the color.
    public override float S => 0.273f; //Saturation
    public override float V => 1.3f; //Brightness

    //Alternatively, leave these values at 1 and provide a custom frame image.
    // TODO Load separate card frames depending on card type
    /*public override Texture2D CustomFrame(CustomCardModel card)
    {
        //This will attempt to load Runesmith2/images/cards/frame.png
        return PreloadManager.Cache.GetTexture2D("cards/frame.png".ImagePath());
    }*/

    //Color of small card icons
    public override Color DeckEntryCardColor => new("97815e");

    public override Color EnergyOutlineColor => new("5b4a31");

    public override bool IsColorless => false;
}