using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tabholic.Utils;
using TabInfo.Utils;
using UIManager;
using UIManager.UI;
using UIManager.UI.Layouts;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

namespace Tabholic.UI {
    public class DestroyAction : MonoBehaviour {
        public Action OnDestroyAction;

        public void Destroy() {
            OnDestroyAction?.Invoke();
        }
    }

    public class TabholicManager : MonoBehaviour {
        public static TabholicManager Instance { get; private set; }

        public static UILayoutPanel LayoutPanel => UIRegistry.Instance.GetLayoutPanel(Tabholic.PanelInfo.PanelID);

        public static List<StatCategoryWrapper> categoryWrappers = new List<StatCategoryWrapper>();

        public static List<PreviewStatCategoryWrapper> previewCategoryWrappers = new List<PreviewStatCategoryWrapper>() {
            new PreviewStatCategoryWrapper("Basic Stats", new PreviewStatWrapper[] {
                new PreviewStatWrapper("HP", "100/100"),
                new PreviewStatWrapper("Damage", "55"),
                new PreviewStatWrapper("Block Cooldown", "4.00s"),
                new PreviewStatWrapper("Reload Time", "2.00s"),
                new PreviewStatWrapper("Ammo", "3"),
                new PreviewStatWrapper("Movement Speed", "1.00")
            }),
            new PreviewStatCategoryWrapper("Advanced Character Stats", new PreviewStatWrapper[] {
                new PreviewStatWrapper("Phoenix Revives", "0"),
                new PreviewStatWrapper("Decay", "0"),
                new PreviewStatWrapper("Regeneration", "0HP/s"),
                new PreviewStatWrapper("Size", "1"),
                new PreviewStatWrapper("Jumps", "1"),
                new PreviewStatWrapper("Jump Height", "1"),
                new PreviewStatWrapper("Gravity", "1")
            })
        };

        public Player Player { get; private set; }
        public bool InitializedCategories { get; private set; }

        private bool InGame = false;

        private void Awake() {
            Instance = this;

            GameModeManager.AddHook(GameModeHooks.HookGameStart, GameStart);
            GameModeManager.AddHook(GameModeHooks.HookGameEnd, GameEnd);
        }

        private void Update() {
            if(!InGame) return;

            if(Input.GetKeyDown(TabholicOptionMenu.ToggleKeyBind.Value) && Player != null) {
                LayoutPanel.Info.DisplayMode = LayoutPanel.Info.DisplayMode == DisplayMode.Normal ? DisplayMode.Hidden : DisplayMode.Normal;
            }
        }

        internal void InitCategories() {
            categoryWrappers = TabInfoManager.Categories.Values.Select(c => new StatCategoryWrapper(c)).OrderBy(c => c.Priority).ToList();
            InitializedCategories = true;
        }

        private IEnumerator GameStart(IGameModeHandler gameModeHandler) {
            if(!InitializedCategories)
                InitCategories();

            SetLocalPlayer();
            InGame = true;

            yield break;
        }

        private IEnumerator GameEnd(IGameModeHandler gameModeHandler) {
            ClearPlayer();
            InGame = false;

            yield break;
        }

        public void SetLocalPlayer() {
            Player player = PlayerManager.instance.players.Find(p => p.data.view.IsMine && !p.GetComponent<PlayerAPI>().enabled && UnboundLib.GameModes.GameModeManager.CurrentHandlerID != UnboundLib.GameModes.GameModeManager.SandBoxID);
            if(player != null) {
                DispalyPlayer(player);
            } else {
                ClearPlayer();
            }
        }


        public void DispalyPlayer(Player player) {
            if(Player != null || player == null) return;

            Player = player;
            Player.gameObject.GetOrAddComponent<DestroyAction>().OnDestroyAction += ClearPlayer;

            LayoutPanel.Info.DisplayMode = DisplayMode.Normal;
        }

        public void ClearPlayer() {
            if(Player == null) return;

            Player.gameObject.GetOrAddComponent<DestroyAction>().OnDestroyAction -= ClearPlayer;
            Player = null;

            LayoutPanel.Info.DisplayMode = DisplayMode.Hidden;
        }
    }
}
