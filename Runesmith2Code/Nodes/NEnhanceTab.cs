using BaseLib.Extensions;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Nodes;

[GlobalClass]
public partial class NEnhanceTab : TextureRect
{
    private static readonly (Color, Color, Color) FontColor = (new Color("f4e8c7"), new Color("00000030"),
        new Color("554c36"));

    private MegaLabel? _enhanceLabel;

    public NCard? NCard { get; private set; }

    public NEnhanceTab WithData(NCard nCard)
    {
        NCard = nCard;

        return this;
    }

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.Center, true);
        Size = new Vector2(162, 40);
        Position = new Vector2(-81, -171);

        if (_enhanceLabel != null) return;
        _enhanceLabel = new MegaLabel();
        _enhanceLabel.MaxFontSize = 17;
        _enhanceLabel.AutoSizeEnabled = true;
        _enhanceLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _enhanceLabel.VerticalAlignment = VerticalAlignment.Center;
        _enhanceLabel.Size = new Vector2(140, 26);
        _enhanceLabel.Position = new Vector2(11, 10);
        _enhanceLabel.AddThemeColorOverride("font_color", FontColor.Item1);
        _enhanceLabel.AddThemeColorOverride("font_shadow_color", FontColor.Item2);
        _enhanceLabel.AddThemeColorOverride("font_outline_color", FontColor.Item3);
        _enhanceLabel.AddThemeConstantOverride("shadow_offset_x", 1);
        _enhanceLabel.AddThemeConstantOverride("shadow_offset_y", 1);
        _enhanceLabel.AddThemeConstantOverride("outline_size", 9);
        _enhanceLabel.AddThemeConstantOverride("shadow_outline_size", 9);
        _enhanceLabel.AddThemeFontOverride("font", BaseResourceIndex.FontKreonRegularSpaceOne);
        _enhanceLabel.Text = "";

        AddChild(_enhanceLabel);
    }

    public void OnEnhanceChanged()
    {
        UpdateEnhanceVisuals();
    }

    public void UpdateEnhanceVisuals()
    {
        // TODO vfx
        if (NCard?._model != null)
        {
            var modifier = NCard._model.GetCardModelModifier();
            if (modifier.Enhanced > 0)
            {
                if (!Visible) Visible = true;
                var locString = RunesmithHoverTipFactory.StaticBanner(RunesmithHoverTip.Enhanced,
                    new DynamicVar("Amount", modifier.Enhanced));
                _enhanceLabel?.SetTextAutoSize(locString.GetFormattedText());
                return;
            }
        }

        Visible = false;
        if (_enhanceLabel != null) _enhanceLabel.Text = "";
    }

    public void OnStasisChanged()
    {
        // TODO
    }
}