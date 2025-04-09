using HarmonyLib;

namespace Hors;

[HarmonyPatch]

public class GameManagerPatches
{
    [HarmonyPatch(typeof(HideAndSeekManager), nameof(HideAndSeekManager.GetBodyType), typeof(PlayerControl))]
    [HarmonyPrefix]

    public static bool Prefix(ref PlayerBodyTypes __result, ref PlayerControl player)
    {
        if (player == null || player.Data == null || player.Data.Role == null)
        {
            if (HorsPlugin.HorsMode.Value)
            {
                __result = PlayerBodyTypes.Horse;
                return false;
            }
            __result = PlayerBodyTypes.Normal;
        }
        else if (HorsPlugin.HorsMode.Value)
        {
            if (player.Data.Role.IsImpostor)
            {
                __result = PlayerBodyTypes.Normal;
                return false;
            }
            __result = PlayerBodyTypes.Horse;
        }
        else
        {
            if (player.Data.Role.IsImpostor)
            {
                __result = PlayerBodyTypes.Seeker;
                return false;
            }
            __result = PlayerBodyTypes.Normal;
        }

        return false;
    }

    [HarmonyPatch(typeof(HideAndSeekManager), nameof(HideAndSeekManager.DeadBodyPrefab), MethodType.Getter)]
    [HarmonyPrefix]

    public static bool Prefix(HideAndSeekManager __instance, ref DeadBody __result)
    {
        if (!HorsPlugin.HorsMode.Value)
        {
            __result = __instance.deadBodyPrefab;
            return false;
        }

        __result = __instance.horseBody;
        return false;
    }

    [HarmonyPatch(typeof(HideAndSeekManager), nameof(HideAndSeekManager.SetSpecialCosmetics), typeof(PlayerControl))]
    [HarmonyPrefix]

    public static bool Prefix(HideAndSeekManager __instance, ref PlayerControl p)
    {
        if (HorsPlugin.HorsMode.Value)
        {
            __instance.horseWranglerOutfit.ColorId = p.CurrentOutfit.ColorId;
            __instance.horseWranglerOutfit.PlayerName = p.CurrentOutfit.PlayerName;
            p.SetOutfit(__instance.horseWranglerOutfit, PlayerOutfitType.HorseWrangler);
        }

        return false;
    }

    [HarmonyPatch(typeof(NormalGameManager), nameof(NormalGameManager.GetBodyType), typeof(PlayerControl))]
    [HarmonyPrefix]

    public static bool Prefix2(ref PlayerBodyTypes __result)
    {
        if (HorsPlugin.HorsMode.Value)
        {
            __result = PlayerBodyTypes.Horse;
            return false;
        }

        __result = PlayerBodyTypes.Normal;
        return false;
    }
}