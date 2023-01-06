using UnityEngine;
using UnityEngine.UI;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;

namespace aitsf2fix;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{

    internal static Harmony Harmony { get; } = new Harmony(MyPluginInfo.PLUGIN_GUID);

    private static ConfigEntry<bool> HighMSAA;
    private static ConfigEntry<int> DesiredResolutionX;
    private static ConfigEntry<int> DesiredResolutionY;
    private static ConfigEntry<bool> Fullscreen;
    private static ConfigEntry<bool> ResolutionOverride;
    private static ConfigEntry<bool> UIFix;
    
    private static BepInEx.Logging.ManualLogSource logger; // Jank???

    public override void Load()
    {
        logger = Log;
        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        HighMSAA = Config.Bind("Quality", "HighMSAA", true, "Enable High Quality MSAA on all cameras");
        
        DesiredResolutionX = Config.Bind("Resolution", "X", Display.main.systemWidth);
        DesiredResolutionY = Config.Bind("Resolution", "Y", Display.main.systemHeight);
        ResolutionOverride = Config.Bind("Resolution", "Override", false);
        Fullscreen = Config.Bind("Resolution", "Fullscreen", false);
        UIFix = Config.Bind("Resolution", "UIFix", true);

        Harmony.PatchAll(typeof(Plugin));
    }

    [HarmonyPatch(typeof(Game.CameraController), nameof(Game.CameraController.OnEnable))]
    [HarmonyPostfix]
    public static void CameraQualityFix(Game.CameraController __instance) {
        if (HighMSAA.Value) {
            var URPD = __instance._camera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            URPD.antialiasing = UnityEngine.Rendering.Universal.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            URPD.antialiasingQuality = UnityEngine.Rendering.Universal.AntialiasingQuality.High;
            // logger.LogInfo("Set Camera to MSAA High");
        }
    }

    [HarmonyPatch(typeof(Game.LauncherArgs), nameof(Game.LauncherArgs.OnRuntimeMethodLoad))]
    public static void SetResolution() {
        if (ResolutionOverride.Value) {
            logger.LogInfo("Overriding resolution");
            if (Fullscreen.Value) {
                Screen.SetResolution(DesiredResolutionX.Value, DesiredResolutionY.Value, FullScreenMode.FullScreenWindow);
                logger.LogInfo("Override to fullscreen");
            } else {
                Screen.SetResolution(DesiredResolutionX.Value, DesiredResolutionY.Value, FullScreenMode.Windowed);
                logger.LogInfo("Override to windowed");
            }
        }
    }
}
