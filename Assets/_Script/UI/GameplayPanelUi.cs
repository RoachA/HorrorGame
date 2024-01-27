using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameplayPanelUi : MonoBehaviour
    {
        [SerializeField] private Image _crosshair;
      
        [SerializeField] private Color _passiveColor;
        [SerializeField] private Color _activeColor;

        [Header("Debug Field")]
        [SerializeField] private bool _debugActive = true;
        [SerializeField] private TextMeshProUGUI _actionDebugTxt;
        [SerializeField] private TextMeshProUGUI _focusDebugTxt;
        [SerializeField] private TextMeshProUGUI _playerPosTxt;
        [SerializeField] private TextMeshProUGUI _fps;

        private Sequence m_crossHairSeq;

        private void Start()
        {
            SetCrosshairState(CrosshairStates.InActive);
        }

        private void Update()
        {
            _playerPosTxt.text = ScreenDubegger.PlayerPos;
           _fps.text = ScreenDubegger.Fps;
        }

        public void SetCrosshairState(CrosshairStates state)
        {
            if (_debugActive)
                UpdateInteractionDebugText();
            
            if (state == CrosshairStates.InActive)
            {
                _crosshair.gameObject.SetActive(false);
                return;
            }
            
            _crosshair.gameObject.SetActive(true);
            m_crossHairSeq?.Kill();
            m_crossHairSeq = DOTween.Sequence();

            m_crossHairSeq.Insert(0, _crosshair.transform.DOScale(state == CrosshairStates.InUse ? Vector3.one : Vector3.one * 1.5f, 0.35f).SetEase(Ease.InOutQuad));
            m_crossHairSeq.Insert(0, _crosshair.DOColor(state == CrosshairStates.InUse ? _activeColor : _passiveColor, 0.35f).SetEase(Ease.InOutQuad));
        }

        public void UpdateInteractionDebugText()
        {
            _actionDebugTxt.text = ScreenDubegger._objectUsedDebug;
            _focusDebugTxt.text = ScreenDubegger._objectInFocusDebug;
        }
    }

    public enum CrosshairStates
    {
        InActive = 0,
        Active = 1,
        InUse = 2,
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