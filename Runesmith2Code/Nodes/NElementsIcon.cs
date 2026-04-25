using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Runesmith2.Runesmith2Code.Cards;
using Runesmith2.Runesmith2Code.Utils;

namespace Runesmith2.Runesmith2Code.Nodes;

public partial class NElementsIcon : TextureRect
{
    private Font _font = BaseResourceIndex.FontKreonBoldShared;

    private MegaLabel[] _labels = new MegaLabel[3];

    private TextureRect[] _unplayableIcons = new TextureRect[3];

    public NCard? NCard { get; private set; }

    public NElementsIcon WithData(NCard nCard)
    {
        NCard = nCard;

        return this;
    }

    public override void _Ready()
    {
        _labels =
        [
            CreateLabel(NElementsCounter.RedFontColor),
            CreateLabel(NElementsCounter.GreenFontColor),
            CreateLabel(NElementsCounter.BlueFontColor)
        ];
        _unplayableIcons =
        [
            CreateUnplayableIcon(),
            CreateUnplayableIcon(),
            CreateUnplayableIcon()
        ];
        for (var i = 0; i < 3; i++)
        {
            _labels[i].AddChild(_unplayableIcons[i]);
            _unplayableIcons[i].Position = new Vector2(-4, 0);
            AddChild(_labels[i]);
        }

        _labels[0].Position = new Vector2(34, 17);
        _labels[1].Position = new Vector2(55, 49);
        _labels[2].Position = new Vector2(13, 49);
    }

    public void UpdateElementsCostVisuals(PileType pileType)
    {
        if (NCard!.Visibility != ModelVisibility.Visible || NCard.Model is not Runesmith2Card runesmithCard)
        {
            for (var i = 0; i < 3; i++)
            {
                var label = _labels[i];
                label.SetTextAutoSize(string.Empty);
                var fontColor = GetFontColor(i);
                label.AddThemeColorOverride(ThemeConstants.Label.FontColor, fontColor.Item1);
                label.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, fontColor.Item3);
            }

            Visible = false;
            return;
        }

        var elementsCost = runesmithCard.GetElementsCostWithModifiers();
        for (var i = 0; i < 3; i++)
        {
            var label = _labels[i];
            label.SetTextAutoSize(elementsCost.ByIndex(i).ToString());
            UpdateElementsCostColor(pileType, runesmithCard, i);
        }

        Visible = runesmithCard.GetElementsCostWithModifiers().Total >= 0;

        var shouldShowUnplayableIcon = false;
        if (pileType == PileType.Hand && !runesmithCard.CanPlay(out var reason, out _))
            shouldShowUnplayableIcon = !reason.HasResourceCostReason();

        foreach (var unplayableIcon in _unplayableIcons) unplayableIcon.Visible = shouldShowUnplayableIcon;
    }

    private void UpdateElementsCostColor(PileType pileType, Runesmith2Card card, int index)
    {
        var (fontColor, _, fontOutlineColor) = GetFontColor(index);

        if (pileType == PileType.Hand)
        {
            var elementsCostColor = RunesmithCardCostHelper.GetElementsCostColor(card, card.CombatState);
            fontColor = NCard.GetCostTextColorInHand(elementsCostColor, NCard!._pretendCardCanBePlayed, fontColor);
            fontOutlineColor =
                NCard.GetCostOutlineColorInHand(elementsCostColor, NCard!._pretendCardCanBePlayed, fontOutlineColor);
        }

        _labels[index].AddThemeColorOverride(ThemeConstants.Label.FontColor, fontColor);
        _labels[index].AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, fontOutlineColor);
    }

    private static (Color, Color, Color) GetFontColor(int index)
    {
        return index switch
        {
            0 => NElementsCounter.RedFontColor,
            1 => NElementsCounter.GreenFontColor,
            2 => NElementsCounter.BlueFontColor,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private MegaLabel CreateLabel((Color, Color, Color) fontColor)
    {
        var label = new MegaLabel();
        label.MaxFontSize = 22;
        label.MinFontSize = 16;
        label.AutoSizeEnabled = false;
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.LayoutMode = 0; //position
        label.Size = new Vector2(28, 36);
        label.SetAnchorsPreset(LayoutPreset.Center);
        label.AddThemeColorOverride("font_color", fontColor.Item1);
        label.AddThemeColorOverride("font_shadow_color", fontColor.Item2);
        label.AddThemeColorOverride("font_outline_color", fontColor.Item3);
        label.AddThemeConstantOverride("shadow_offset_x", 2);
        label.AddThemeConstantOverride("shadow_offset_y", 2);
        label.AddThemeConstantOverride("outline_size", 12);
        label.AddThemeConstantOverride("shadow_outline_size", 12);
        label.AddThemeFontOverride("font", _font);
        label.AddThemeFontSizeOverride("font_size", 22);
        label.Text = "0";

        return label;
    }

    private static TextureRect CreateUnplayableIcon()
    {
        var textRect = new TextureRect();
        textRect.Texture = BaseResourceIndex.CardUnplayableIcon;
        textRect.ExpandMode = ExpandModeEnum.IgnoreSize;
        textRect.Size = new Vector2(36, 36);
        return textRect;
    }
}