#region

using System.Reflection;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Runesmith2.Runesmith2Code.Utils;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

#endregion

namespace Runesmith2.Runesmith2Code;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
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
    }
}