using Tabholic.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tabholic.UI {
    public class StatCategoryFrame : MonoBehaviour {
        public GameObject StatPrefab;
        public GameObject StatsHolder;
        public TextMeshProUGUI Text;

        public VerticalLayoutGroup StatHolderLayoutGroup;
        public VerticalLayoutGroup CategoryHolderLayoutGroup;

        private TabholicFrame TabholicFrame;
        internal IBaseStatCategory StatCategoryWrapper;

        private bool InitializeOptionControl = false;

        private void OnEnable() {
            Display();
        }

        private void Update() {
            Display();
        }

        public void Initialize(TabholicFrame frame, IBaseStatCategory statCategoryWrapper) {
            StatCategoryWrapper = statCategoryWrapper;
            TabholicFrame = frame;

            foreach(IBaseStat obj in StatCategoryWrapper.Stats) {
                GameObject statHolder = GameObject.Instantiate(StatPrefab, StatsHolder.transform);
                statHolder.GetComponent<StatObject>().Initialize(frame, obj);
            }

            if(TabholicManager.Instance.Player != null) {
                bool canDisplay = StatCategoryWrapper.CanDisplay(TabholicManager.Instance.Player);

                gameObject.SetActive(canDisplay);
                if(!canDisplay) {
                    TabholicFrame.DisableStatsCategoriesDisplayers.Add(this);
                }
            }

            Text.text = StatCategoryWrapper.Display();
        }

        public void Display() {
            if(!StatCategoryWrapper.CanDisplay(TabholicManager.Instance.Player)) {
                TabholicFrame.DisableStatsCategoriesDisplayers.Add(this);
                gameObject.SetActive(false);
            }

            if(!InitializeOptionControl && TabholicOptionPanel.Instance != null) {
                RectTransform textRect = Text.GetComponent<RectTransform>();

                Text.gameObject.SetActive(TabholicOptionPanel.Instance.CategoryHeader.Value);
                Text.color = TabholicOptionPanel.Instance.CategoryColor.Value;
                textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, TabholicOptionPanel.Instance.CategoryTextSize.Value);
                StatHolderLayoutGroup.spacing = TabholicOptionPanel.Instance.StatGap.Value;
                CategoryHolderLayoutGroup.spacing = TabholicOptionPanel.Instance.CategoryTextGap.Value;

                TabholicOptionPanel.Instance.CategoryHeader.OnValueChanged += (value) => Text.gameObject.SetActive(value);
                TabholicOptionPanel.Instance.CategoryColor.OnValueChanged += (value) => Text.color = value;
                TabholicOptionPanel.Instance.StatGap.OnValueChanged += (value) => StatHolderLayoutGroup.spacing = value;
                TabholicOptionPanel.Instance.CategoryTextGap.OnValueChanged += (value) => CategoryHolderLayoutGroup.spacing = value;
                TabholicOptionPanel.Instance.CategoryTextSize.OnValueChanged += (value) => textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);

                InitializeOptionControl = true;
            }
        }
    }
}
