using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    [RequireComponent(typeof(Image))]
    public class CrossHairView : MonoBehaviour
    {
        [SerializeField] private Image _crosshair;
        [SerializeField] private Color _passiveColor;
        [SerializeField] private Color _activeColor;
        
        private Sequence m_crossHairSeq;
        
        private void Start()
        {
            SetCrosshairState(CrosshairStates.InActive);
        }
        
        public void SetCrosshairState(CrosshairStates state)
        {
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
    }
}