using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class GWYFMenuPlugin : BaseUnityPlugin
{
    public const string PluginGuid = "thebozzz34.gwyf.menu";
    public const string PluginName = "GWYF Menu";
    public const string PluginVersion = "1.0.0";

    private Harmony _harmony = null!;

    private void Awake()
    {
        _harmony = new Harmony(PluginGuid);
        _harmony.PatchAll();
        Logger.LogInfo("GWYF Menu loaded.");
    }

    private void OnDestroy()
    {
        _harmony?.UnpatchSelf();
    }
}

[HarmonyPatch(typeof(AllPlayersTriggerZone), nameof(AllPlayersTriggerZone.OnStartClient))]
internal static class AllPlayersTriggerZonePatch
{
    private static void Prefix(AllPlayersTriggerZone __instance)
    {
        try
        {
            if (GameObject.Find("__DebugWindow") != null)
            {
                return;
            }

            GameObject gameObject = new GameObject("__DebugWindow");
            Object.DontDestroyOnLoad(gameObject);
            gameObject.AddComponent<DebugWindow>().message = string.Concat(new object[]
            {
                "OnStartClient fired\nScene: ",
                SceneManager.GetActiveScene().name,
                "\nObject: ",
                __instance.gameObject.name,
                "\nisServer: ",
                __instance.isServer.ToString(),
                "\nisClient: ",
                __instance.isClient.ToString(),
                "\nnetId: ",
                __instance.netId
            });
        }
        catch
        {
        }
    }
}
