using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using Runesmith2.Runesmith2Code.Extensions;
using Runesmith2.Runesmith2Code.Structs;
using Runesmith2.Runesmith2Code.Utils;

public partial class NElementsCounter : Control
{
    private static readonly StringName _v = new("v");

    private static readonly StringName _s = new("s");

    public static readonly (Color, Color, Color) RedFontColor = (new Color("ffe3e3"), new Color("00000030"),
        new Color("541111"));

    public static readonly (Color, Color, Color) GreenFontColor = (new Color("e3ffe3"), new Color("00000030"),
        new Color("115411"));

    public static readonly (Color, Color, Color) BlueFontColor = (new Color("e3e3ff"), new Color("00000030"),
        new Color("111154"));

    // TODO VFX for gaining elements

    private Player? _player;

    private Control _rotationLayers;

    private TextureRect _icon;

    private ShaderMaterial _hsv;

    private float _lerpingIgnisCount;

    private float _lerpingTerraCount;

    private float _lerpingAquaCount;

    private float _velocity1;

    private float _velocity2;

    private float _velocity3;

    private Elements _displayedElementsCount = new();

    private MegaLabel[] _labels;

    private Tween? _hsvTween;

    private bool _isListeningToCombatState;

    private HoverTip _hoverTip;

    public void Initialize(Player player)
    {
        _player = player;
        ConnectElementsChangedSignal();
        RefreshVisibility();
    }

    public override void _Ready()
    {
        _rotationLayers = GetNode<Control>("%RotationLayers");
        _icon = GetNode<TextureRect>("%Icon");
        _hsv = (ShaderMaterial)_icon.Material;
        _labels =
        [
            CreateLabel(RedFontColor),
            CreateLabel(GreenFontColor),
            CreateLabel(BlueFontColor)
        ];
        GetNode<MarginContainer>("%MarginContainer1").AddChild(_labels[0]);
        GetNode<MarginContainer>("%MarginContainer2").AddChild(_labels[1]);
        GetNode<MarginContainer>("%MarginContainer3").AddChild(_labels[2]);
        var locString = new LocString("static_hover_tips", "RUNESMITH2-ELEMENTS_COUNT.description");
        locString.Add("ElementsIcon", "[img]res://Runesmith2/images/charui/elements_all_icon.png[/img]");
        locString.Add("IgnisIcon", "[img]res://Runesmith2/images/charui/elements_ignis_icon.png[/img]");
        locString.Add("TerraIcon", "[img]res://Runesmith2/images/charui/elements_terra_icon.png[/img]");
        locString.Add("AquaIcon", "[img]res://Runesmith2/images/charui/elements_aqua_icon.png[/img]");
        _hoverTip = new HoverTip(new LocString("static_hover_tips", "RUNESMITH2-ELEMENTS_COUNT.title"), locString,
            RunesmithResource.ElementsIcon);
        Connect(Control.SignalName.MouseEntered, Callable.From(OnHovered));
        Connect(Control.SignalName.MouseExited, Callable.From(OnUnhovered));
        SetElementsCountText(new Elements(0), true);
        Visible = false;
    }

    private static MegaLabel CreateLabel((Color, Color, Color) fontColor)
    {
        var label = new MegaLabel();
        label.MaxFontSize = 28;
        label.AutoSizeEnabled = false;
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.AddThemeColorOverride("font_color", fontColor.Item1);
        label.AddThemeColorOverride("font_shadow_color", fontColor.Item2);
        label.AddThemeColorOverride("font_outline_color", fontColor.Item3);
        label.AddThemeConstantOverride("shadow_offset_x", 3);
        label.AddThemeConstantOverride("shadow_offset_y", 3);
        label.AddThemeConstantOverride("outline_size", 15);
        label.AddThemeConstantOverride("shadow_outline_size", 15);
        label.AddThemeFontOverride("font", BaseResourceIndex.FontKreonBoldSpaceTwo);
        label.AddThemeFontSizeOverride("font_size", 28);
        label.Text = "0";

        return label;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        ConnectElementsChangedSignal();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_player != null && _isListeningToCombatState)
        {
            var runesmithCombatState = _player.PlayerCombatState?.Runesmith();
            runesmithCombatState?.ElementsChanged -= OnElementsChanged;
            _isListeningToCombatState = false;
        }
    }

    private void ConnectElementsChangedSignal()
    {
        if (_player != null && !_isListeningToCombatState)
        {
            var runesmithCombatState = _player.PlayerCombatState?.Runesmith();
            runesmithCombatState?.ElementsChanged += OnElementsChanged;
            _isListeningToCombatState = true;
        }
    }

    private void OnHovered()
    {
        var nHoverTipSet = NHoverTipSet.CreateAndShow(this, _hoverTip);
        nHoverTipSet?.GlobalPosition = GlobalPosition + new Vector2(-70, -240);
    }

    private void OnUnhovered()
    {
        NHoverTipSet.Remove(this);
    }

    private void OnElementsChanged(Elements oldElements, Elements newElements)
    {
        UpdateElementsCount(oldElements, newElements);
        RefreshVisibility();
    }

    public override void _Process(double delta)
    {
        if (_player == null) return;
        var elements = GetPlayerElements(_player);
        var rotSpeed = elements.Total == 0 ? 10f : 40f;
        for (var i = 0; i < _rotationLayers.GetChildCount(); i++)
            _rotationLayers.GetChild<Control>(i).RotationDegrees += (float)delta * rotSpeed * (i + 1);

        _lerpingIgnisCount =
            MathHelper.SmoothDamp(_lerpingIgnisCount, elements.Ignis, ref _velocity1, 0.1f, (float)delta);
        _lerpingTerraCount =
            MathHelper.SmoothDamp(_lerpingTerraCount, elements.Terra, ref _velocity2, 0.1f, (float)delta);
        _lerpingAquaCount = MathHelper.SmoothDamp(_lerpingAquaCount, elements.Aqua, ref _velocity3, 0.1f, (float)delta);
        SetElementsCountText(new Elements(Mathf.RoundToInt(_lerpingIgnisCount), Mathf.RoundToInt(_lerpingTerraCount),
            Mathf.RoundToInt(_lerpingAquaCount)));
    }

    private static Elements GetPlayerElements(Player player)
    {
        var elements = player.PlayerCombatState?.Elements() ?? new Elements();
        return elements;
    }

    private void UpdateElementsCount(Elements oldCount, Elements newCount)
    {
        // Elements should only go up or down together so there shouldn't be a case where 1 element go up and another go down 
        if (newCount.Total < oldCount.Total)
        {
            _hsvTween?.Kill();
            _hsv.SetShaderParameter(_v, 1f);
            _lerpingIgnisCount = newCount.Ignis;
            _lerpingTerraCount = newCount.Terra;
            _lerpingAquaCount = newCount.Aqua;
            SetElementsCountText(newCount);
        }
        else if (newCount.Total > oldCount.Total)
        {
            _hsvTween?.Kill();
            _hsvTween = CreateTween();
            _hsvTween.TweenMethod(Callable.From<float>(UpdateShaderV), 2f, 1f, 0.2);
            //TODO vfx gain elements
        }
    }

    private void SetElementsCountText(Elements elements, bool initSetup = false)
    {
        if (initSetup || _displayedElementsCount != elements)
        {
            _displayedElementsCount = elements;
            for (var i = 0; i < 3; i++)
            {
                var label = _labels[i];
                var elemValue = elements.ByIndex(i);
                var fontColor = i switch
                {
                    0 => RedFontColor.Item1,
                    1 => GreenFontColor.Item1,
                    2 => BlueFontColor.Item1,
                    _ => throw new ArgumentOutOfRangeException()
                };

                label.AddThemeColorOverride(ThemeConstants.Label.FontColor, elemValue == 0 ? StsColors.red : fontColor);
                label.Text = elemValue.ToString();
            }

            if (elements.Total == 0)
            {
                _hsv.SetShaderParameter(_s, 0.5f);
                _hsv.SetShaderParameter(_v, 0.85f);
            }
            else
            {
                _hsv.SetShaderParameter(_s, 1f);
                _hsv.SetShaderParameter(_v, 1f);
            }
        }
    }

    private void UpdateShaderV(float value)
    {
        _hsv.SetShaderParameter(_v, value);
    }

    private void RefreshVisibility()
    {
        if (_player == null)
        {
            Visible = false;
            return;
        }

        var elements = GetPlayerElements(_player);

        var shouldAlwaysShowElements = _player.Character is Runesmith2.Runesmith2Code.Character.Runesmith2;

        Visible = Visible || shouldAlwaysShowElements || elements.Total > 0;
    }
}