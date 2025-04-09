using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace Hors;

[HarmonyPatch]

public class killAnimationPatch
{
    [HarmonyPatch(typeof(KillOverlay), nameof(KillOverlay.ShowKillAnimation), typeof(NetworkedPlayerInfo), typeof(NetworkedPlayerInfo))]
    [HarmonyPrefix]

    public static bool Prefix(KillOverlay __instance, ref NetworkedPlayerInfo killer, ref NetworkedPlayerInfo victim)
    {
        Il2CppReferenceArray<OverlayKillAnimation> enumerable = __instance.KillAnims;
        OverlayKillAnimation overlayKillAnimation;
        if (killer.Object)
        {
            OverlayKillAnimation[] killAnimations = killer.Object.MyPhysics.Animations.GetKillAnimations();
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek && HorsPlugin.HorsMode.Value)
            {
                enumerable = __instance.HorseWrangleAnims;
            }
            else if (killAnimations.Length != 0)
            {
                enumerable = killAnimations;
            }
            else
            {
                SkinViewData skin = ShipStatus.Instance.CosmeticsCache.GetSkin(killer.Object.CurrentOutfit.SkinId);
                if (skin && skin.CustomKillAnimID > 0)
                {
                    if (!string.IsNullOrEmpty(skin.HatID_KillAnim) && skin.HatID_KillAnim != killer.Object.CurrentOutfit.HatId)
                    {
                        overlayKillAnimation = enumerable[Random.Range(0, enumerable.Count)];
                        __instance.ShowKillAnimation(overlayKillAnimation, killer, victim);
                        return false;
                    }
                    if (!string.IsNullOrEmpty(skin.VisorID_KillAnim) && skin.VisorID_KillAnim != killer.Object.CurrentOutfit.VisorId)
                    {
                        overlayKillAnimation = enumerable[Random.Range(0, enumerable.Count)];
                        __instance.ShowKillAnimation(overlayKillAnimation, killer, victim);
                        return false;
                    }
                    enumerable = new Il2CppReferenceArray<OverlayKillAnimation>([__instance.CustomKillAnimations[skin.CustomKillAnimID - 1]]);
                }
            }
        }
        overlayKillAnimation = enumerable[Random.Range(0, enumerable.Count)];
        __instance.ShowKillAnimation(overlayKillAnimation, killer, victim);

        return false;
    }
}