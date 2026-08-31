using BaseLib.Config;

namespace Runesmith2.Runesmith2Code.Utils;

public class RunesmithConfig : SimpleModConfig
{
    [ConfigHoverTip] public static bool UploadMetrics { get; set; } = false;

    [ConfigHideInUI]
    [ConfigIgnoreRestoreDefaults]
    public static bool UploadMetricsFtueSeen { get; set; } = false;

    [ConfigSection("VisualAndSoundEffects")]
    [ConfigHoverTip]
    public static bool EnableGrindstoneVfx { get; set; } = true;

    public static bool EnableGrindstoneSfx { get; set; } = true;
    [ConfigHoverTip] public static bool EnableStasisVfx { get; set; } = true;
    public static bool EnableStasisSfx { get; set; } = true;
    [ConfigHoverTip] public static bool EnableEnhanceVfx { get; set; } = true;
    public static bool EnableEnhanceSfx { get; set; } = true;
    public static bool EnableRuneChargeSfx { get; set; } = true;
    public static bool EnableElementsGainSfx { get; set; } = true;
    public static bool EnableLaserTurretSfx { get; set; } = true;
}