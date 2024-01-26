using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameplayPanelUi : MonoBehaviour
    {
        [SerializeField] private Image _crosshair;
      
        [SerializeField] private Color _passiveColor;
        [SerializeField] private Color _activeColor;

        private Sequence m_crossHairSeq;

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

            m_crossHairSeq.Insert(0, _crosshair.transform.DOScale(state == CrosshairStates.Active ? Vector3.one * 1.5f : Vector3.one, 0.35f).SetEase(Ease.InOutQuad));
            m_crossHairSeq.Insert(0, _crosshair.DOColor(state == CrosshairStates.Active ? _activeColor : _passiveColor, 0.35f).SetEase(Ease.InOutQuad));
        }
    }

    public enum CrosshairStates
    {
        InActive = 0,
        Active = 1,
        InUse = 2,
    }
}