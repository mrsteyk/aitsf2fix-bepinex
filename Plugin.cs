using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    private static ConfigEntry<bool> NoVSync;
    private static ConfigEntry<int> Aniso;
    private static ConfigEntry<int> DesiredResolutionX;
    private static ConfigEntry<int> DesiredResolutionY;
    private static ConfigEntry<bool> Fullscreen;
    private static ConfigEntry<bool> ResolutionOverride;
    private static ConfigEntry<bool> UIFix;
    
    public static BepInEx.Logging.ManualLogSource logger; // Jank???

    public override void Load()
    {
        logger = base.Log;
        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        HighMSAA = Config.Bind("Quality", "HighMSAA", true, "Enable High Quality MSAA on all cameras");
        NoVSync = Config.Bind("Quality", "NoVSync", false, "Force NoVSync setting");
        Aniso = Config.Bind("Quality", "ForceAniso", -1, "Force this anisoLevel");
        
        DesiredResolutionX = Config.Bind("Resolution", "X", Display.main.systemWidth);
        DesiredResolutionY = Config.Bind("Resolution", "Y", Display.main.systemHeight);
        ResolutionOverride = Config.Bind("Resolution", "Override", false);
        Fullscreen = Config.Bind("Resolution", "Fullscreen", false);
        UIFix = Config.Bind("Resolution", "UIFix", true);

        Harmony.PatchAll(typeof(Plugin));
        Harmony.PatchAll(typeof(TopMenu));
        Harmony.PatchAll(typeof(FlagViewerFix));
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

    [HarmonyPatch(typeof(UnityEngine.Texture), "get_anisoLevel")]
    [HarmonyPrefix]
    public static void Texture2Dctor(UnityEngine.Texture __instance) {
        if (Aniso.Value >= 0)
            __instance.anisoLevel = Aniso.Value;
    }

    [HarmonyPatch(typeof(Game.LauncherArgs), nameof(Game.LauncherArgs.OnRuntimeMethodLoad))]
    [HarmonyPostfix]
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
        logger.LogInfo("Applying stuff!");
        // Get's set to 8 :pensive:
        // Get's set to 0 if I do it here :face_melt:
        if (HighMSAA.Value)
            UnityEngine.QualitySettings.antiAliasing = 16;
        // UnityEngine.QualitySettings.anisotropicFiltering = UnityEngine.AnisotropicFiltering.ForceEnable;
        // foreach(var name in UnityEngine.QualitySettings.names) {
        //     logger.LogInfo(name);
        // }
        // There's only one quality level 0 - Fastest named "High"???
        UnityEngine.QualitySettings.SetQualityLevel((int)UnityEngine.QualityLevel.Fantastic, true);
        // Clamps to 9-16
        UnityEngine.QualitySettings.anisotropicFiltering = UnityEngine.AnisotropicFiltering.ForceEnable;
        UnityEngine.QualitySettings.shadowResolution = UnityEngine.ShadowResolution.VeryHigh;
        UnityEngine.QualitySettings.skinWeights = UnityEngine.SkinWeights.Unlimited;
        UnityEngine.QualitySettings.softParticles = true;
        if (NoVSync.Value)
            UnityEngine.QualitySettings.vSyncCount = 0;
    }

    // [HarmonyPatch(typeof(Develop.DevelopManager), "get_UserName")]
    // public static string get_UserName() {
    //     logger.LogInfo(System.Environment.StackTrace);
    //     return "gonzo";
    // }

    [HarmonyPatch(typeof(FolderManager), nameof(FolderManager.GetFiles))]
    [HarmonyPostfix]
    public static void GetFiles(string path, ref Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStringArray __result) {
        // logger.LogInfo(path);
        var arr = BustShots.consts.Where(c => c.Key == path).ToArray();
        if (arr.Length > 0) {
            __result = arr.Select(c => c.Value).ToArray();
        } else if (path.StartsWith("Assets/Asset/graphics/3d/chara/motion/mot_")) {
            // logger.LogError(path);
            /*
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c06_03/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c52_01/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c20_01/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c30_02/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c30_02/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c20_03/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c75_01/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c20_06/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c24_03/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
[Error  : aitsf2fix] Assets/Asset/graphics/3d/chara/motion/mot_c00_00/face
            */
        }
    }
}
