using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI
{
    public enum CrosshairStates
    {
        InActive = 0,
        Active = 1,
        InUse = 2,
    }
    
    public class GameplayPanelUi : MonoBehaviour
    {
        [Inject] private readonly SignalBus m_bus;
        [BoxGroup("UI Items")]
        [SerializeField] private InventoryPanel _inventoryPanel;
        [SerializeField] private CrossHairView _crosshairView;

        [Space]
        [Header("Debug Field")]
        [SerializeField] private bool _debugActive = true;
        [SerializeField] private TextMeshProUGUI _actionDebugTxt;
        [SerializeField] private TextMeshProUGUI _focusDebugTxt;
        [SerializeField] private TextMeshProUGUI _playerPosTxt;
        [SerializeField] private TextMeshProUGUI _fps;

        private List<UiPanel> _activePanels = new List<UiPanel>(); 

        public void OpenPanel<T>() where T : UiPanel
        {
            if (_activePanels == null) _activePanels = new List<UiPanel>();
            
            if (typeof(T) == typeof(InventoryPanel))
            {
                _activePanels.Add(_inventoryPanel);
                _inventoryPanel.Open();
            }
            
            m_bus.Fire(new CoreSignals.OverwriteMouseLookSensitivitySignal(0.05f));
            m_bus.Fire(new CoreSignals.SetCursorSignal(true));
        }

        public bool TryClosePanel<T>() where T : UiPanel
        {
            if (TryGetPanelOfType(out T panel))
            {
                panel.Close();
                _activePanels.Remove(panel);
                m_bus.Fire(new CoreSignals.OverwriteMouseLookSensitivitySignal(1f));
                m_bus.Fire(new CoreSignals.SetCursorSignal(false));
                return true;
            }

            return false;
        }
        
        bool TryGetPanelOfType<T>(out T panel) where T : UiPanel
        {
            panel = _activePanels.OfType<T>().FirstOrDefault();
            return panel != null;
        }

        public void UpdateCrosshair(CrosshairStates state)
        {
            if (_debugActive)
                UpdateInteractionDebugText();
            
            _crosshairView.SetCrosshairState(state);
        }

        private void Update()
        {
            _playerPosTxt.text = ScreenDubegger.PlayerPos;
           _fps.text = ScreenDubegger.Fps;
        }
        
        public void UpdateInteractionDebugText()
        {
            _actionDebugTxt.text = ScreenDubegger._objectUsedDebug;
            _focusDebugTxt.text = ScreenDubegger._objectInFocusDebug;
        }
    }
    
    public static class ScreenDubegger
    {
        public static string PlayerPos => SetPlayerPos(PlayerController.Instance.transform.position);
        public static string Fps;
        public static string _objectUsedDebug;
        public static string _objectInFocusDebug;

        private static string SetPlayerPos(Vector3 pos)
        {
            return "X: " + pos.x + " Y: " + pos.y;
        }
    }
    
    
}