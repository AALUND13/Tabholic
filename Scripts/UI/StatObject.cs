using Tabholic.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tabholic.UI {
    public class StatObject : MonoBehaviour {
        public TextMeshProUGUI Text;
        private TabholicFrame TabholicFrame;

        internal IBaseStat StatWrapper;

        private bool InitializeOptionControl = false;

        private void Awake() {
            TabholicFrame = GetComponentInParent<TabholicFrame>();
        }

        private void OnEnable() {
            Display();

            foreach(VerticalLayoutGroup verticalLayoutGroup in GetComponentsInParent<VerticalLayoutGroup>()) {
                LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.transform as RectTransform);
            }
        }

        private void Update() {
            Display();
        }


        public void Initialize(TabholicFrame frame, IBaseStat statCategoryWrapper) {
            StatWrapper = statCategoryWrapper;
            TabholicFrame = frame;

            if(TabholicManager.Instance.Player != null) {
                bool canDisplay = StatWrapper.CanDisplay(TabholicManager.Instance.Player);

                gameObject.SetActive(canDisplay);
                if(!canDisplay) {
                    TabholicFrame.DisableStatsDisplayers.Add(this);
                }
            }
        }

        public void Display() {
            if(!StatWrapper.CanDisplay(TabholicManager.Instance.Player)) {
                TabholicFrame.DisableStatsDisplayers.Add(this);
                gameObject.SetActive(false);
                
                foreach(VerticalLayoutGroup verticalLayoutGroup in GetComponentsInParent<VerticalLayoutGroup>()) {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(verticalLayoutGroup.transform as RectTransform);
                }
            } else {
                Text.text = StatWrapper.Display(TabholicManager.Instance.Player);
            }

            if(!InitializeOptionControl && TabholicOptionPanel.Instance != null) {
                Text.color = TabholicOptionPanel.Instance.StatColor.Value;

                TabholicOptionPanel.Instance.StatColor.OnValueChanged += (value) => Text.color = value;

                InitializeOptionControl = true;
            }
        }
    }
}
