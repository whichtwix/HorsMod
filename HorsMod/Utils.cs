

using System.Collections;
using System.Linq;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections.Generic;
using PowerTools;
using UnityEngine;

namespace Hors;

public class Utils
{
    public static IEnumerator CoBeginReplacement(IntroCutscene __instance)
	{
		Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Starting intro cutscene", null);
		SoundManager.Instance.PlaySound(__instance.IntroStinger, false, 1f, null);
		if (GameManager.Instance.IsNormal())
		{
			Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Game Mode: Normal", null);
			__instance.LogPlayerRoleData();
			__instance.HideAndSeekPanels.SetActive(false);
			__instance.CrewmateRules.SetActive(false);
			__instance.ImpostorRules.SetActive(false);
			__instance.ImpostorName.gameObject.SetActive(false);
			__instance.ImpostorTitle.gameObject.SetActive(false);
            var @delegate = DelegateSupport.ConvertDelegate<Il2CppSystem.Func<NetworkedPlayerInfo, bool>>((NetworkedPlayerInfo pcd) => !PlayerControl.LocalPlayer.Data.Role.IsImpostor || pcd.Role.TeamType == PlayerControl.LocalPlayer.Data.Role.TeamType);
			List<PlayerControl> list = IntroCutscene.SelectTeamToShow(@delegate);
			if (list == null || list.Count < 1)
			{
				Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: teamToShow is EMPTY or NULL", null);
			}
			if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
			{
				__instance.ImpostorText.gameObject.SetActive(false);
			}
			else
			{
				int adjustedNumImpostors = GameManager.Instance.LogicOptions.GetAdjustedNumImpostors(GameData.Instance.PlayerCount);
				if (adjustedNumImpostors == 1)
				{
					__instance.ImpostorText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsS, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
				}
				else
				{
					__instance.ImpostorText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsP, new Il2CppReferenceArray<Il2CppSystem.Object>([adjustedNumImpostors]));
				}
				__instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[FF1919FF]", "<color=#FF1919FF>");
				__instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[]", "</color>");
			}
			yield return __instance.StartCoroutine(__instance.ShowTeam(list, 3f));
			yield return __instance.StartCoroutine(__instance.ShowRole());
		}
		else
		{
			Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Game Mode: Hide and Seek", null);
			__instance.LogPlayerRoleData();
			__instance.HideAndSeekPanels.SetActive(true);
			if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
			{
				__instance.CrewmateRules.SetActive(false);
				__instance.ImpostorRules.SetActive(true);
			}
			else
			{
				__instance.CrewmateRules.SetActive(true);
				__instance.ImpostorRules.SetActive(false);
			}
            var @delegate = DelegateSupport.ConvertDelegate<Il2CppSystem.Func<NetworkedPlayerInfo, bool>>((NetworkedPlayerInfo pcd) => PlayerControl.LocalPlayer.Data.Role.IsImpostor != pcd.Role.IsImpostor) ;
			List<PlayerControl> list2 = IntroCutscene.SelectTeamToShow(@delegate);
			if (list2 == null || list2.Count < 1)
			{
				Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: teamToShow is EMPTY or NULL", null);
			}
			PlayerControl impostor = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault((PlayerControl pc) => pc.Data.Role.IsImpostor);
			if (impostor == null)
			{
				Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: impostor is NULL", null);
			}
			GameManager.Instance.SetSpecialCosmetics(impostor);
			__instance.ImpostorName.gameObject.SetActive(true);
			__instance.ImpostorTitle.gameObject.SetActive(true);
			__instance.BackgroundBar.enabled = false;
			__instance.TeamTitle.gameObject.SetActive(false);
			if (impostor != null)
			{
				__instance.ImpostorName.text = impostor.Data.PlayerName;
			}
			else
			{
				__instance.ImpostorName.text = "???";
			}
			yield return new WaitForSecondsRealtime(0.1f);
			PoolablePlayer playerSlot = null;
			if (impostor != null)
			{
				playerSlot = __instance.CreatePlayer(1, 1, impostor.Data, false);
				playerSlot.SetBodyType(PlayerBodyTypes.Normal);
				playerSlot.SetFlipX(false);
				playerSlot.transform.localPosition = __instance.impostorPos;
				playerSlot.transform.localScale = Vector3.one * __instance.impostorScale;
			}
			yield return __instance.StartCoroutine(ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers());
			yield return new WaitForSecondsRealtime(6f);
			if (playerSlot != null)
			{
				playerSlot.gameObject.SetActive(false);
			}
			__instance.HideAndSeekPanels.SetActive(false);
			__instance.CrewmateRules.SetActive(false);
			__instance.ImpostorRules.SetActive(false);
			LogicOptionsHnS logicOptionsHnS = GameManager.Instance.LogicOptions.TryCast<LogicOptionsHnS>();
			LogicHnSMusic logicHnSMusic = GameManager.Instance.GetLogicComponent<LogicHnSMusic>().TryCast<LogicHnSMusic>();
			if (logicHnSMusic != null)
			{
				logicHnSMusic.StartMusicWithIntro();
			}
			if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
			{
				float crewmateLeadTime = logicOptionsHnS.GetCrewmateLeadTime();
				__instance.HideAndSeekTimerText.gameObject.SetActive(true);
				PoolablePlayer poolablePlayer;
				AnimationClip animationClip;
				if (HorsPlugin.HorsMode.Value)
				{
					poolablePlayer = __instance.HorseWrangleVisualSuit;
					poolablePlayer.gameObject.SetActive(true);
					poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
					animationClip = __instance.HnSSeekerSpawnHorseAnim;
					__instance.HorseWrangleVisualPlayer.SetBodyType(PlayerBodyTypes.Normal);
					__instance.HorseWrangleVisualPlayer.UpdateFromPlayerData(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.CurrentOutfitType, PlayerMaterial.MaskType.None, false, null, false);
				}
				else
				{
					poolablePlayer = __instance.HideAndSeekPlayerVisual;
					poolablePlayer.gameObject.SetActive(true);
					poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
					animationClip = __instance.HnSSeekerSpawnAnim;
				}
				poolablePlayer.SetBodyCosmeticsVisible(false);
				poolablePlayer.UpdateFromPlayerData(PlayerControl.LocalPlayer.Data, PlayerControl.LocalPlayer.CurrentOutfitType, PlayerMaterial.MaskType.None, false, null, false);
				SpriteAnim component = poolablePlayer.GetComponent<SpriteAnim>();
				poolablePlayer.gameObject.SetActive(true);
				poolablePlayer.ToggleName(false);
				component.Play(animationClip, 1f);
				while (crewmateLeadTime > 0f)
				{
					__instance.HideAndSeekTimerText.text = Mathf.RoundToInt(crewmateLeadTime).ToString();
					crewmateLeadTime -= Time.deltaTime;
					yield return null;
				}
			}
			else
			{
				ShipStatus.Instance.HideCountdown = logicOptionsHnS.GetCrewmateLeadTime();
				if (HorsPlugin.HorsMode.Value)
				{
					if (impostor != null)
					{
						impostor.AnimateCustom(__instance.HnSSeekerSpawnHorseInGameAnim);
					}
				}
				else if (impostor != null)
				{
					impostor.AnimateCustom(__instance.HnSSeekerSpawnAnim);
					impostor.cosmetics.SetBodyCosmeticsVisible(false);
				}
			}
			impostor = null;
			playerSlot = null;
		}
		ShipStatus.Instance.StartSFX();
		Object.Destroy(__instance.gameObject);
		yield break;
	}
}