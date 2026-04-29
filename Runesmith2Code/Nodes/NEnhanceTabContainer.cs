#region

using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.HoverTips;
using Runesmith2.Runesmith2Code.Utils;

#endregion

namespace Runesmith2.Runesmith2Code.Nodes;

[GlobalClass]
public partial class NEnhanceTabContainer : Control
{
    // TODO refactor this to Font color utils class
    private static readonly (Color, Color, Color) FontColor = (new Color("f4e8c7"), new Color("00000030"),
        new Color("554c36"));

    private static readonly (Color, Color, Color) StasisFontColor = (new Color("cef2f2"), new Color("00000030"),
        new Color("303a55"));

    private static readonly Vector3 BlueHsv = new(0.478f, 1.574f, 1.0f);

    private ShaderMaterial _hsv;

    private TextureRect _enhanceTab;

    private TextureRect _stasisOverlay;

    private MegaLabel? _enhanceLabel;

    public NCard? NCard { get; private set; }

    public NEnhanceTabContainer WithData(NCard nCard)
    {
        NCard = nCard;

        return this;
    }

    public override void _Ready()
    {
        SetAnchorsPreset(LayoutPreset.Center, true);
        Size = new Vector2(162, 40);
        Position = new Vector2(-81, -171);

        _enhanceTab = GetNode<TextureRect>("%EnhanceTab");
        _enhanceTab.Visible = false;
        _hsv = (ShaderMaterial)_enhanceTab.Material;

        _stasisOverlay = GetNode<TextureRect>("%StasisOverlay");
        _stasisOverlay.Visible = false;

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

        _enhanceLabel.UseParentMaterial = true;

        AddChild(_enhanceLabel);
    }


    public void OnEnhanceChanged()
    {
        UpdateEnhanceVisuals();
    }

    public void UpdateEnhanceVisuals()
    {
        if (!IsNodeReady()) return;
        if (NCard?._model != null)
        {
            var modifier = NCard._model.GetCardModelModifier();

            if (modifier.Enhanced > 0)
            {
                if (!_enhanceTab.Visible) _enhanceTab.Visible = true;
                var locString = RunesmithHoverTipFactory.StaticBanner(RunesmithHoverTip.Enhanced,
                    new DynamicVar("Amount", modifier.Enhanced));
                _enhanceLabel?.SetTextAutoSize(locString.GetFormattedText());
            }
            else
            {
                _enhanceTab.Visible = false;
                _enhanceLabel?.Text = "";
            }

            if (modifier.Stasis)
            {
                UpdateShaderHsv(BlueHsv);
                _enhanceLabel?.AddThemeColorOverride(ThemeConstants.Label.FontColor, StasisFontColor.Item1);
                _enhanceLabel?.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, StasisFontColor.Item3);
                _stasisOverlay.Visible = true;
            }
            else
            {
                UpdateShaderHsv(Vector3.One);
                _enhanceLabel?.AddThemeColorOverride(ThemeConstants.Label.FontColor, FontColor.Item1);
                _enhanceLabel?.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, FontColor.Item3);
                _stasisOverlay.Visible = false;
            }

            return;
        }

        Visible = false;
    }

    private void UpdateShaderHsv(Vector3 vec)
    {
        _hsv.SetShaderParameter("h", vec.X);
        _hsv.SetShaderParameter("s", vec.Y);
        _hsv.SetShaderParameter("v", vec.Z);
    }

    public void OnStasisChanged()
    {
        UpdateEnhanceVisuals();
    }
}