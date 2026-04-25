using BaseLib.Utils.Patching;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using Runesmith2.Runesmith2Code.Field;

namespace Runesmith2.Runesmith2Code.Patches;

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.MutableClone))]
internal class AbstractModelMutableClonePatch
{
    private static void CloneSpireField(AbstractModel src, AbstractModel dest)
    {
        if (src is not CardModel srcModel || dest is not CardModel destModel) return;
        var modifier = RunesmithField.Modifier[srcModel];
        if (modifier == null) return;
        RunesmithField.Modifier[destModel] = modifier.Clone(destModel);
    }

    [HarmonyTranspiler]
    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions).Match(new InstructionMatcher()
            .ldloc_0()
            .ldc_i4_1()
            .callvirt(typeof(AbstractModel), "set_IsMutable", [typeof(bool)])
        ).Step(-3).Insert([
            CodeInstruction.LoadArgument(0),
            CodeInstruction.LoadLocal(0),
            CodeInstruction.Call(typeof(AbstractModelMutableClonePatch), nameof(CloneSpireField))
        ]);
    }
}