using HarmonyLib;
using Develop;
using TMPro;

using BepInEx;

namespace aitsf2fix;

public class TopMenu {
    public static bool aging_active = false;
    public static bool debug_active = false;
    // public static DevelopManager Instance = null;
    // public static Game.RootManager rooti = null;
    // public static TextMeshProUGUI fpstext = null;

    // [HarmonyPatch(typeof(Game.RootManager), nameof(Game.RootManager.Awake))]
    // [HarmonyPostfix]
    // public static void RootAwake(Game.RootManager __instance) {
    //     rooti = __instance;
    // }

    [HarmonyPatch(typeof(DevelopManager), nameof(DevelopManager.Awake))]
    [HarmonyPostfix]
    public static void Awake(DevelopManager __instance) {
        // Hide the shitter
        UnityEngine.GameObject.Find("_root/#Canvas/SafeArea/InGameMenu/RightPane/Windows/EventCall").SetActive(false);

        // In post all static stuff should be set...
        // Utter retardation idk how to cast in this language
        // Instance = __instance;
        __instance.topMenuButton.onPointerClick += new System.Action<UnityEngine.EventSystems.PointerEventData>((e) => {
            SDevelopManager.Instance.ShowMenu();
        });
        __instance.debugInfoButton.onPointerClick += new System.Action<UnityEngine.EventSystems.PointerEventData>((e) => {
            debug_active = !debug_active;
            // I can't get normal to work so I am recreating it...
            DevelopSaveData.develop.display = debug_active;
            // AAAAAAAAAAAAAAAAAAaaaaaaaaaaaaaaaaaaaaaaaa
            (new DevelopManager(SDevelopManager.Instance.Pointer)).inGameMenu.SetActive(debug_active);
        });
        __instance.motionButton.onPointerClick += new System.Action<UnityEngine.EventSystems.PointerEventData>((e) => {
            SDevelopManager.Instance.ToggleMotion();
        });
        __instance.agingButton.onPointerClick += new System.Action<UnityEngine.EventSystems.PointerEventData>((e) => {
            // ???
            aging_active = !aging_active;
            Game.SAgingManager.Instance.SetActive(aging_active);
        });
        __instance.resetButton.onPointerClick += new System.Action<UnityEngine.EventSystems.PointerEventData>((e) => {
            // This barely works tbh...
            SDevelopManager.Instance.Reset();
            // Maybe this?
            // This resets Static BGViewer or something like that
            // Game.RootManager.StaticReset();
            (new Game.RootManager(Game.SRootManager.Instance.Pointer)).Restart();
        });
        __instance.saveLoadButton.onPointerClick += new System.Action<UnityEngine.EventSystems.PointerEventData>((e) => {
            // ???
            // This is joystick input related
            // Instance.SaveLoadKey();
            var i = new DevelopManager(SDevelopManager.Instance.Pointer);
            Game.SSaveDataManager.Instance.Save(i.saveloadMode.ToString());
            Game.SSaveDataManager.Instance.Load(i.saveloadMode.ToString());
        });
        for(var i = 0; i<5; i++) {
            __instance.saveButtons[i].onPointerClick += new System.Action<UnityEngine.EventSystems.PointerEventData>((e) => {
                Game.SSaveDataManager.Instance.Save(i.ToString());
            });
            __instance.loadButtons[i].onPointerClick += new System.Action<UnityEngine.EventSystems.PointerEventData>((e) => {
                Game.SSaveDataManager.Instance.Load(i.ToString());
            });
        }
        __instance.menuButton.onPointerClick += new System.Action<UnityEngine.EventSystems.PointerEventData>((e) => {
            // TODO: blergh?
            // (new Game.RootManager(Game.SRootManager.Instance.Pointer)).JumpTitle();
            // This should be more correct?
            var i = new DevelopManager(SDevelopManager.Instance.Pointer);
            var c = i.developCanvasGroup;
            if (c.alpha != 1) {
                c.alpha = 1;
                c.blocksRaycasts = true;
            } else {
                c.alpha = 0;
                c.blocksRaycasts = false;
            }
        });

        // fpstext = UnityEngine.GameObject.Find("_root/#Canvas/SafeArea/TopButtons/FPS/Text").GetComponent<TextMeshProUGUI>();
        // Plugin.logger.LogInfo(fpstext);
    }

    // Dead locks the game after a while...
    // UniFPSCounter doesn't have anything other than a constructor redefined...
    [HarmonyPatch(typeof(Game.RootManager), nameof(Game.RootManager.Update))]
    [HarmonyPostfix]
    public static void Update() {
        // try {
        //     if (fpstext)
        //         fpstext.SetText((1.0f/UnityEngine.Time.unscaledDeltaTime).ToString("F1"), true);
        // } catch {
        //     try {
        //         fpstext = UnityEngine.GameObject.Find("_root/#Canvas/SafeArea/TopButtons/FPS/Text").GetComponent<TextMeshProUGUI>();
        //     } catch {}
        // }
        if(FlagViewerFix.dirty_flag) {
            if (FlagViewerFix.Instance) {
                if (FlagViewerFix.Instance.enabled && FlagViewerFix.Instance.gameObject.active) {
                    FlagViewerFix.dirty_flag = false;
                    FlagViewerFix.hkReload();
                }
            }
        }
    }
}