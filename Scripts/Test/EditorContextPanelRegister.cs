using Tabholic.UI;
using UIManager.UI;
using UIManager.UI.ContextPanels;
using UnityEngine;

namespace Tabholic.UI.Tests {
    // This is for testing the "Tabholic" UI panel directly in the "Unity Editor"
    public class EditorContextPanelRegister : MonoBehaviour {
        public UIPanelInfo PrefabPanelInfo;

        private void Awake() {
            OptionPanelRegistry.RegisterOptionMenu<TabholicOptionPanel>(PrefabPanelInfo);
        }
    }
}
