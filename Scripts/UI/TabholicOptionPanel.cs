using UIManager.Properties;
using UIManager.UI.ContextPanels.SimplePanels;
using UnityEngine;

namespace Tabholic.UI {
    public class TabholicOptionPanel : SimpleContextPanel {
        public static TabholicOptionPanel Instance { get; private set; }

        [Header("Enable/Disable Options")]
        public BoolPropertyField CategoryHeader
            = new BoolPropertyField(true);

        [Header("Spacing")]
        public FloatPropertyField CategoryGap 
            = new FloatPropertyField(10f, 20f, 0f);
        public FloatPropertyField CategoryTextGap 
            = new FloatPropertyField(2f, 10f, 0f);
        public FloatPropertyField StatGap 
            = new FloatPropertyField(1f, 10f, 0f);

        [Header("Color")]
        public ColorPropertyField CategoryColor 
            = new ColorPropertyField(new Color(0.72f, 0.72f, 0.72f, 1f));
        public ColorPropertyField StatColor 
            = new ColorPropertyField(new Color(0.72f, 0.72f, 0.72f, 1f));

        [Header("Scaling")]
        public FloatPropertyField PanelScale 
            = new FloatPropertyField(1f, 2f, 0.75f);
        public FloatPropertyField CategoryTextSize 
            = new FloatPropertyField(20f, 20f, 10f);

        private void Awake() {
            Instance = this;
        }
    }
}
