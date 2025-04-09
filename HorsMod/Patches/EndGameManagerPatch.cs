using System.Linq;
using AmongUs.Data;
using AmongUs.GameOptions;
using Assets.CoreScripts;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace Hors;

public class EndGameManagerPatch
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.SetEverythingUp))]
    [HarmonyPrefix]

    public static bool Prefix(EndGameManager __instance)
    {
        DataManager.Player.Stats.IncrementStat(StatID.GamesFinished);
        __instance.Navigation.HideButtons();
        bool flag = GameManager.Instance.DidHumansWin(EndGameResult.CachedGameOverReason);
        if (EndGameResult.CachedGameOverReason == GameOverReason.ImpostorDisconnect)
        {
            DataManager.Player.Stats.IncrementGameResultStat(EndGameResult.CachedGameOverReason, GameResultStat.Draws);
            __instance.WinText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.ImpostorDisconnected, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
            SoundManager.Instance.PlaySound(__instance.DisconnectStinger, false, 1f, null);
        }
        else
        {
            if (EndGameResult.CachedWinners.ToArray().Any(h => h.IsYou))
            {
                DataManager.Player.Stats.IncrementWinStats(EndGameResult.CachedGameOverReason, (MapNames)GameManager.Instance.LogicOptions.MapId, EndGameResult.CachedLocalPlayer.RoleWhenAlive);
                DestroyableSingleton<AchievementManager>.Instance.SetWinMap(GameManager.Instance.LogicOptions.MapId);
                __instance.WinText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Victory, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
                CachedPlayerData cachedPlayerData = EndGameResult.CachedWinners.ToArray().FirstOrDefault(h => h.IsYou);
                if (cachedPlayerData != null)
                {
                    DestroyableSingleton<UnityTelemetry>.Instance.WonGame(cachedPlayerData.ColorId, cachedPlayerData.HatId, cachedPlayerData.SkinId, cachedPlayerData.PetId, cachedPlayerData.VisorId, cachedPlayerData.NamePlateId);
                }
            }
            else
            {
                DataManager.Player.Stats.IncrementGameResultStat(EndGameResult.CachedGameOverReason, GameResultStat.Losses);
                __instance.WinText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Defeat, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                __instance.WinText.color = Color.red;
            }
            if (flag)
            {
                SoundManager.Instance.PlaySound(__instance.CrewStinger, false);
            }
            else
            {
                SoundManager.Instance.PlaySound(__instance.ImpostorStinger, false);
            }
        }
        int num = Mathf.CeilToInt(7.5f);
        List<CachedPlayerData> list = [.. EndGameResult.CachedWinners.ToArray().OrderBy(delegate(CachedPlayerData b)
            {
                if (!b.IsYou)
                {
                    return 0;
                }
                return -1;
            })];
        for (int i = 0; i < list.Count; i++)
        {
            CachedPlayerData cachedPlayerData2 = list[i];
            int num2 = (i % 2 == 0) ? (-1) : 1;
            int num3 = (i + 1) / 2;
            float num4 = num3 / (float)num;
            float num5 = Mathf.Lerp(1f, 0.75f, num4);
            float num6 = (i == 0) ? (-8) : (-1);
            PoolablePlayer poolablePlayer = Object.Instantiate(__instance.PlayerPrefab, __instance.transform);
            poolablePlayer.transform.localPosition = new Vector3(1f * num2 * num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + num3 * 0.01f) * 0.9f;
            float num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
            Vector3 vector;
            vector = new(num7, num7, 1f);
            poolablePlayer.transform.localScale = vector;
            if (cachedPlayerData2.IsDead)
            {
                poolablePlayer.SetBodyAsGhost();
                poolablePlayer.SetDeadFlipX(i % 2 == 0);
            }
            else
            {
                poolablePlayer.SetFlipX(i % 2 == 0);
            }
            poolablePlayer.UpdateFromPlayerOutfit(cachedPlayerData2.Outfit, PlayerMaterial.MaskType.None, cachedPlayerData2.IsDead, true, null, false);
            if (flag)
            {
                poolablePlayer.ToggleName(false);
            }
            else
            {
                Color color = cachedPlayerData2.IsImpostor ? Palette.ImpostorRed : Palette.White;
                poolablePlayer.SetName(cachedPlayerData2.PlayerName, vector.Inv(), color, -15f);
                Vector3 vector2;
                vector2 = new(0f, -1.31f, -0.5f);
                poolablePlayer.SetNamePosition(vector2);
                if (HorsPlugin.HorsMode.Value && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                {
                    poolablePlayer.SetBodyType(PlayerBodyTypes.Normal);
                    poolablePlayer.SetFlipX(false);
                }
            }
        }

        return false;
    }
}