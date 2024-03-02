using Game.World.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class InventoryItemView : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private GameObject _activeIndicator;
        [SerializeField] private Button _btn;

        private InventoryPanel m_parentPanel;
        private ObtainableItemData m_data;
        
        
        public void Init(IObtainable item, InventoryPanel panel)
        {
            if (item.Data == null)
            {
                Debug.LogError(item.Object.name + " has no data assigned to it, cannot be drawn in inventory");
                return;
            }

            m_parentPanel = panel;
            m_data = item.Data;
            _image.sprite = m_data._inventorySprite;
            _activeIndicator.SetActive(false);
        }

        public void Reset()
        {
            _image.sprite = null;
            m_data = null;
        }

        private void OnDestroy()
        {
            _btn.onClick.RemoveListener(OnClicked);
        }

        public void SetActiveState(bool state)
        {
            _activeIndicator.SetActive(state); 
        }

        private void Start()
        {
            if (_btn == null) return;
            
            _btn.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            m_parentPanel.OnInventoryItemSelected(this);
        }
    }
}