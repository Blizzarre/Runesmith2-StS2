#region

using System.Reflection;
using BaseLib.Audio;
using BaseLib.Config;
using BaseLib.Utils;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Runesmith2.Runesmith2Code.Data;
using Runesmith2.Runesmith2Code.Utils;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

#endregion

namespace Runesmith2.Runesmith2Code;

[ModInitializer(nameof(Initialize))]
public partial class Runesmith2Mod : Node
{
    public const string ModId = "Runesmith2"; //Used for resource filepath

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();

        ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());

        RunesmithSubscriber.Subscribe();

        ModManager.OnMetricsUpload += Runesmith2Metrics.OnMetricsUpload;

        ModConfigRegistry.Register(ModId, new RunesmithConfig());

        ModSound.SetSoundDefaultVolumeOffset("res://Runesmith2/audio/runesmith_character_transition.ogg", 32f);
        ModSound.SetSoundDefaultVolumeOffset("res://Runesmith2/audio/runesmith_character_select.ogg", 32f);
    }


    public static string GetVersion()
    {
        var mod = ModManager.GetLoadedMods().FirstOrDefault(m => m.manifest?.id == "Runesmith2");

        return mod?.manifest?.version ?? "unknown";
    }
}