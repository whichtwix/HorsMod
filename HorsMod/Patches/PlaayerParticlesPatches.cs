using HarmonyLib;
using UnityEngine;

namespace Hors;

[HarmonyPatch]

public class PlayerParticlesPatches
{
    [HarmonyPatch(typeof(PlayerParticles), nameof(PlayerParticles.Start))]
    [HarmonyPrefix]

    public static bool Prefix(PlayerParticles __instance)
    {
        __instance.fill = new RandomFill<PlayerParticleInfo>();
        if (HorsPlugin.HorsMode.Value)
        {
            __instance.fill.values = __instance.HorseSprites;
            __instance.pool.Prefab = __instance.HorsePrefab;
        }
        else
        {
            __instance.fill.values = __instance.Sprites;
        }
        int num = 0;
        while (__instance.pool.NotInUse > 0)
        {
            PlayerParticle playerParticle = __instance.pool.Get<PlayerParticle>();
            PlayerMaterial.SetColors(num++, playerParticle.myRend);
            __instance.PlacePlayer(playerParticle, true);
        }
        return false;
    }

    [HarmonyPatch(typeof(PlayerParticles), nameof(PlayerParticles.PlacePlayer))]
    [HarmonyPrefix]

    public static bool Prefix(PlayerParticles __instance, ref PlayerParticle part, ref bool initial)
    {
        Vector3 vector = Random.insideUnitCircle;
        if (!initial)
        {
            vector.Normalize();
        }
        vector *= __instance.StartRadius;
        float num = __instance.scale.Next();
        if (HorsPlugin.HorsMode.Value)
        {
            num *= 0.7f;
        }
        part.transform.localScale = new Vector3(num, num, num);
        vector.z = -num * 0.001f;
        if (__instance.FollowCamera)
        {
            Vector3 position = __instance.FollowCamera.transform.position;
            position.z = 0f;
            vector += position;
            part.FollowCamera = __instance.FollowCamera;
        }
        part.transform.localPosition = vector;
        PlayerParticleInfo playerParticleInfo = __instance.fill.Get();
        part.myRend.sprite = playerParticleInfo.image;
        part.myRend.flipX = BoolRange.Next(0.5f);
        Vector2 vector2 = -vector.normalized;
        vector2 = vector2.Rotate(FloatRange.Next(-45f, 45f));
        part.velocity = vector2 * __instance.velocity.Next();
        part.angularVelocity = playerParticleInfo.angularVel.Next();
        if (playerParticleInfo.alignToVel)
        {
            part.transform.localEulerAngles = new Vector3(0f, 0f, Vector2.up.AngleSigned(vector2));
        }

        return false;
    }
}