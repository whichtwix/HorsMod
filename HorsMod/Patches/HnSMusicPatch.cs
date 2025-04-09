using HarmonyLib;
using UnityEngine;

namespace Hors;

[HarmonyPatch]

public class MusicPatch
{
    [HarmonyPatch(typeof(LogicHnSMusic), nameof(LogicHnSMusic.StartMusicWithIntro))]
    [HarmonyPrefix]

    public static bool Prefix(LogicHnSMusic __instance)
    {
        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        {
            AudioClip audioClip = (__instance.Manager.TryCast<HideAndSeekManager>().LogicOptionsHnS.GetEscapeTime() <= 180f) ? __instance.musicCollection.ImpostorShortMusic : __instance.musicCollection.ImpostorLongMusic;
            if (HorsPlugin.HorsMode.Value)
            {
                audioClip = __instance.musicCollection.ImpostorRanchMusic;
            }
            SoundManager.Instance.PlaySound(audioClip, true, 1f, SoundManager.Instance.MusicChannel);
        }

        return false;
    }
}