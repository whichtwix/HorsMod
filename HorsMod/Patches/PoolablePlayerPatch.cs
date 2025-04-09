using HarmonyLib;

namespace Hors;

[HarmonyPatch]

public class PoolablePlayerPatch
{
    [HarmonyPatch(typeof(PoolablePlayer), nameof(PoolablePlayer.InitBody))]
    [HarmonyPrefix]

    public static bool Prefix(PoolablePlayer __instance)
    {
        if (HorsPlugin.HorsMode.Value)
        {
            __instance.bodyType = PlayerBodyTypes.Horse;
        }
        else
        {
            __instance.bodyType = PlayerBodyTypes.Normal;
        }
        __instance.cosmetics.EnsureInitialized(__instance.bodyType);

        return false;
    }
}