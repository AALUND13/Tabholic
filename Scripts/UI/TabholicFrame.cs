using System.Collections.Generic;
using Tabholic.Utils;
using UIManager.UI;
using UIManager.UI.Layouts;
using UnityEngine;
using UnityEngine.UI;

namespace Tabholic.UI {
    public class TabholicFrame : MonoBehaviour {
        public GameObject CategoryPrefeb;

        public List<StatCategoryFrame> Frames = new List<StatCategoryFrame>();

        public List<StatObject> DisableStatsDisplayers = new List<StatObject>();
        public List<StatCategoryFrame> DisableStatsCategoriesDisplayers = new List<StatCategoryFrame>();

        public VerticalLayoutGroup CategoryLayoutGroup;

        private bool Preview = false;
        private bool First = true;

        private bool InitializeOptionControl = false;

        private void Awake() {
            UIPanelInfo panel = GetComponentInParent<UIPanelInfo>();
            if(First) {
                panel.OnDisplayModeChnaged += (DisplayMode display) => {
                    if(display == DisplayMode.Normal) {
                        CreatePlayerStats();
                    } else if(display == DisplayMode.Layout) {
                        CreatePreviewStats();
                    }
                };
                
                if(panel.DisplayMode == DisplayMode.Normal) {
                    CreatePlayerStats();
                } else if(panel.DisplayMode == DisplayMode.Layout) {
                    CreatePreviewStats();
                }

                First = false;
            }
        }

        private void Update() {
            foreach(StatObject stat in new List<StatObject>(DisableStatsDisplayers)) {
                if(stat.StatWrapper.CanDisplay(TabholicManager.Instance.Player)) {
                    DisableStatsDisplayers.Remove(stat);
                    stat.gameObject.SetActive(true);
                }
            }

            foreach(StatCategoryFrame category in new List<StatCategoryFrame>(DisableStatsCategoriesDisplayers)) {
                if(category.StatCategoryWrapper.CanDisplay(TabholicManager.Instance.Player)) {
                    DisableStatsCategoriesDisplayers.Remove(category);
                    category.gameObject.SetActive(true);
                }
            }


            if(!InitializeOptionControl && TabholicOptionPanel.Instance != null) {
                transform.localScale = Vector3.one * TabholicOptionPanel.Instance.PanelScale.Value;
                CategoryLayoutGroup.spacing = TabholicOptionPanel.Instance.CategoryGap.Value;

                TabholicOptionPanel.Instance.PanelScale.OnValueChanged += (value) => transform.localScale = Vector3.one * value;
                TabholicOptionPanel.Instance.CategoryGap.OnValueChanged += (value) => CategoryLayoutGroup.spacing = value;
            }
        }

        private void CreatePreviewStats() {
            Preview = true;

            foreach(var frame in Frames) {
                GameObject.Destroy(frame.gameObject);
            }

            Frames.Clear();
            DisableStatsCategoriesDisplayers.Clear();
            DisableStatsDisplayers.Clear();

            foreach(IBaseStatCategory statCategory in TabholicManager.previewCategoryWrappers) {
                GameObject statHolder = GameObject.Instantiate(CategoryPrefeb, transform);
                StatCategoryFrame StatCategoryFrame = statHolder.GetComponent<StatCategoryFrame>();
                StatCategoryFrame.Initialize(this, statCategory);
                
                if(TabholicManager.Instance.Player != null) {
                    StatCategoryFrame.gameObject.SetActive(StatCategoryFrame.StatCategoryWrapper.CanDisplay(TabholicManager.Instance.Player));
                }

                Frames.Add(StatCategoryFrame);
            }
        }

        private void CreatePlayerStats() {
            Preview = false;

            foreach(var frame in Frames) {
                GameObject.Destroy(frame.gameObject);
            }

            Frames.Clear();
            DisableStatsCategoriesDisplayers.Clear();
            DisableStatsDisplayers.Clear();

            foreach(IBaseStatCategory statCategory in TabholicManager.categoryWrappers) {
                GameObject statHolder = GameObject.Instantiate(CategoryPrefeb, transform);
                StatCategoryFrame StatCategoryFrame = statHolder.GetComponent<StatCategoryFrame>();
                StatCategoryFrame.Initialize(this, statCategory);

                Frames.Add(StatCategoryFrame);
            }
        }
    }
}
