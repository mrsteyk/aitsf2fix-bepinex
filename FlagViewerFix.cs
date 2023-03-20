using Unity;
using UnityEngine;
using HarmonyLib;

using Develop;

namespace aitsf2fix;

public class FlagViewerFix { // MonoBehaviour, IFlagViewer, IBaseBehaviour {
    public static FlagViewer Instance = null;
    public static TreeView tree = null;
    public static Content rootContent = null;
    public static bool dirty_flag = false;

    public static System.IntPtr klass_Content = Il2CppInterop.Runtime.IL2CPP.GetIl2CppClass("Assembly-CSharp.dll", "Develop", "Content");

    [HarmonyPatch(typeof(Develop.FlagViewer), nameof(Develop.FlagViewer.Awake))]
    [HarmonyPostfix]
    public static void hkAwake(Develop.FlagViewer __instance) {
        // Plugin.logger.LogInfo("HK AWAKE CALLED!");
        Instance = __instance;
        tree = Instance.GetComponent<Develop.TreeView>();
        Plugin.logger.LogInfo(tree);
        tree.SetTitle("FlagViewer", "");
        tree.persist = DevelopSaveData.develop.flagViewer;
        dirty_flag = true;
    }

    [HarmonyPatch(typeof(Develop.FlagViewer), nameof(Develop.FlagViewer.OnDestroy))]
    [HarmonyPostfix]
    public static void hkDestroy() {
        Instance = null;
        tree = null;
    }

    // [HarmonyPatch(typeof(Develop.FlagViewer), nameof(Develop.FlagViewer.ReloadFlag))]
    // [HarmonyPostfix]
    public static void hkReload() {
        // Hide the shitter
        UnityEngine.GameObject.Find("_root/#Canvas/SafeArea/InGameMenu/RightPane/Windows/EventCall").SetActive(false);

        // I trully don't know if that's needed...
        rootContent = new Develop.Content(Il2CppInterop.Runtime.IL2CPP.il2cpp_object_new(klass_Content));
        // rootContent.level = -1;

        if (Game.SFlagManager.Instance == null) {
            dirty_flag = true;
            return;
        }

        var mem = Game.SFlagManager.Instance.GetMembers();
        if (mem.count == 0) {
            dirty_flag = true;
            return;
        }
        foreach(var e in mem) {
            if (e.Value.FieldType.ToString() == "System.Boolean") {
                var checkbox = new Develop.Content(Il2CppInterop.Runtime.IL2CPP.il2cpp_object_new(klass_Content));
                checkbox.name = e.Key;
                // checkbox.value = e.Value.GetValue();
                checkbox.value = Game.SFlagManager.Instance.Get(e.Key);
                checkbox.onValueChanged += new System.Action<Content, Il2CppSystem.Object>((c, o) => {Game.SFlagManager.Instance.Set(c.name, o); c.value = o;});
                rootContent.AddChild(checkbox);
            } else {
                Plugin.logger.LogError("Unknown type " + e.Value.FieldType.ToString());
            }
        }

        tree.SetList(rootContent);
    }

    // Sadly I can't override methods not overriden already...
}