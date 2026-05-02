using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tabholic.UI;
using UIManager;
using UIManager.UI;
using UIManager.UI.ContextPanels;
using UnboundLib;
using UnityEngine;

namespace Tabholic {
    [BepInDependency("com.willis.rounds.unbound")]
    [BepInDependency("com.willuwontu.rounds.tabinfo")]

    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class Tabholic : BaseUnityPlugin {
        internal const string ModId = "AALUND13.ROUNDS.Tabholic";
        internal const string ModName = "Tabholic";
        internal const string Version = "1.0.0";

        public static Tabholic Instance { get; private set; }
        public static AssetBundle Assets { get; private set; }

        public static UIPanelInfo PanelInfo { get; internal set; }

        private static Harmony Harmony;

        private void Awake() {
            Instance = this;

            Unbound.RegisterClientSideMod(ModId);

            Assets = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("tabholic_assets", typeof(Tabholic).Assembly);

            Harmony = new Harmony(ModId);
            Harmony.PatchAll();
        }

        private void Start() {
            UIPanelRegistrar uIPanelRegistrar = Assets.LoadAsset<GameObject>("Tabholic UI Regsiter").GetComponent<UIPanelRegistrar>();
            uIPanelRegistrar.Register();

            PanelInfo = uIPanelRegistrar.UIPanelsPrefabs.First();

            OptionPanelRegistry.RegisterOptionMenu<TabholicOptionPanel>(PanelInfo);
            gameObject.AddComponent<TabholicManager>();
            TabholicOptionMenu.RegisterMenu();
        }

        private void Update() {
            TabholicOptionMenu.OnUpdate();
        }
    }
}
