using System.Linq;
using Tabholic.UI;
using Tabholic.Utils;
using TMPro;
using UIManager;
using UIManager.Menus;
using UIManager.Utils.Configs;
using UnboundLib;
using UnboundLib.Utils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tabholic {
    public static class TabholicOptionMenu {
        public static EnumGlobleConfig<KeyCode> ToggleKeyBind = new EnumGlobleConfig<KeyCode>("show_tabholic_keybind", KeyCode.O);
        public static BoolGlobleConfig SimplisticMode = new BoolGlobleConfig("tabholic_simplistic_mode", true);

        private static TextMeshProUGUI KeyText;
        private static GameObject KeybindButton;

        private static bool DetectKey;

        internal static void RegisterMenu() {
            Unbound.RegisterMenu("Tabholic", () => { }, NewGui, null, false);
        }

        private static void NewGui(GameObject menu) {
            if(!TabholicManager.Instance.InitializedCategories) TabholicManager.Instance.InitCategories();

            MenuHandler.CreateText(Tabholic.ModName, menu, out _, 90, false, null, null, null, null);
            MenuHandler.CreateButton("Manage Tabholic Panel", menu, () => UILayoutMenu.Instance.Open(Tabholic.PanelInfo));
            MenuHandler.CreateText(" ", menu, out _, 60, false, null, null, null, null);

            MenuHandler.CreateText("Keybinds", menu, out _, 75, false, null, null, null, null);
            KeybindButton = MenuHandler.CreateButton("Set Toggle Keybind", menu, () => {
                DetectKey = true;
                UpdateKeyText();
            }, 35, true, null, null, null, null);
            MenuHandler.CreateText($"Current Toggle Keybind: {ToggleKeyBind.Value.ToString()}", menu, out KeyText, 40, true, null, null, null, null);
            MenuHandler.CreateText(" ", menu, out _, 60, false, null, null, null, null);

            MenuHandler.CreateText("Toggles", menu, out _, 75, false, null, null, null, null);
            MenuHandler.CreateToggle(SimplisticMode.Value, "Simplistic Mode", menu, (value) => {
                SimplisticMode.Value = value;
            });
            MenuHandler.CreateText("<#7A7A7A><i>\"Simplistic Mode\" Only Show Category/Stats That is Enable in The \"Categories\" Section", menu, out _, 35, false, null, null, null, null);
            MenuHandler.CreateText(" ", menu, out _, 60, false, null, null, null, null);

            MenuHandler.CreateText("Categories", menu, out _, 75, false, null, null, null, null);
            foreach(StatCategoryWrapper categoryWrapper in TabholicManager.categoryWrappers) {
                CreateCategoryGUI(MenuHandler.CreateMenu(categoryWrapper.WrappingCategory.name, () => { }, menu, 60, true, true, menu.transform.parent.gameObject), categoryWrapper);
            }
        }

        private static void UpdateKeyText() {
            if(DetectKey) {
                KeyText.text = "Current Toggle Keybind: Press Any Key...";
            } else {
                KeyText.text = $"Current Toggle Keybind: {ToggleKeyBind.Value.ToString()}";
            }
        }

        private static void CreateCategoryGUI(GameObject menu, StatCategoryWrapper categoryWrapper) {
            MenuHandler.CreateText(categoryWrapper.WrappingCategory.name.ToUpper(), menu, out _, 90, false, null, null, null, null);
            MenuHandler.CreateText(" ", menu, out _, 30, false, null, null, null, null);

            MenuHandler.CreateText("Toggle Category", menu, out _, 60, false, null, null, null, null);
            MenuHandler.CreateText(" ", menu, out _, 30, false, null, null, null, null);

            Toggle categoryToggle = MenuHandler.CreateToggle(categoryWrapper.Enabled.Value, $"{categoryWrapper.WrappingCategory.name}", menu, (value) => categoryWrapper.Enabled.Value = value, 60)
                                    .GetComponentInChildren<Toggle>();
            

            MenuHandler.CreateText(" ", menu, out _, 30, false, null, null, null, null);

            MenuHandler.CreateText("Toggle Stats ", menu, out _, 60, false, null, null, null, null);
            MenuHandler.CreateText(" ", menu, out _, 30, false, null, null, null, null);

            foreach(StatWrapper statWrapper in categoryWrapper.Stats.Select(s => (StatWrapper)s)) {
                CreateCategoryGUI(menu, categoryToggle, categoryWrapper, statWrapper);
            }
        }

        private static void CreateCategoryGUI(GameObject menu, Toggle categoryToggle, StatCategoryWrapper categoryWrapper, StatWrapper statWrapper) {
            MenuHandler.CreateToggle(statWrapper.Enabled.Value, $"{categoryWrapper.WrappingCategory.name} {statWrapper.WrappingStat.name}", menu, (value) => {
                if(value) categoryToggle.isOn = true;
                statWrapper.Enabled.Value = value;
            }, 60);
        }

        internal static void OnUpdate() {
            if(KeybindButton == null || KeyText == null)
                return;

            if(DetectKey) {
                if(Input.GetKeyDown(KeyCode.Escape)) {
                    DetectKey = false;
                    UpdateKeyText();
                    return;
                }

                foreach(KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode))) {
                    if(Input.GetKeyDown(keyCode)) {
                        DetectKey = false;
                        ToggleKeyBind.Value = keyCode;
                        UpdateKeyText();
                    }
                }
            }
        }
    }
}
