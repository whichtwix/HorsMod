using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace Hors;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]

public partial class HorsPlugin : BasePlugin
{
    public Harmony Harmony { get; } = new(Id);

    public static ConfigEntry<bool> HorsMode { get; private set; }

    public override void Load()
    {
        HorsMode = Config.Bind("Hors", "HorsMode", true, "Enable Hors mode.");
        Harmony.PatchAll();
    }
}
