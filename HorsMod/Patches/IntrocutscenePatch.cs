using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;

namespace Hors;

[HarmonyPatch]

public class IntroCutscenePatch
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.CoBegin))]
    [HarmonyPrefix]

    public static bool Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        __result = Utils.CoBeginReplacement(__instance).WrapToIl2Cpp();
        return false;
    }
}