using HarmonyLib;
using Hors;

[HarmonyPatch]

public class AccountTabPatch
{
    [HarmonyPatch(typeof(AccountTab), nameof(AccountTab.Awake))]
    [HarmonyPrefix]

    public static bool Prefix(AccountTab __instance)
    {
        __instance.SpaceHorse.enabled = HorsPlugin.HorsMode.Value;
        __instance.SpaceBean.enabled = !HorsPlugin.HorsMode.Value;

        return false;
    }
}