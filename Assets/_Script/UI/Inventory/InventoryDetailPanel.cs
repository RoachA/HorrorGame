using Game.World.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class InventoryDetailParams : UIPanelParams
    {
        public ReadableItemData ItemData;
        public bool ReturnToIventory;

        public InventoryDetailParams(ReadableItemData itemData, bool returnToIventory = false)
        {
            ItemData = itemData;
            ReturnToIventory = returnToIventory;
        }
    }
    
    public class InventoryDetailPanel : UiPanel
    {
        [SerializeField] private Image _art;
        [SerializeField] private TextMeshProUGUI _descTxt;
        [SerializeField] private TextMeshProUGUI _headerTxt;

        [SerializeField] private Button _closeButton;

        private bool m_returnToInventory;

        protected override void Init(UIPanelParams data)
        {
            var readableData = (InventoryDetailParams) data;
            _art.sprite = readableData.ItemData._readableAssetSprite;
            _descTxt.text = readableData.ItemData._readableText;
            _headerTxt.text = readableData.ItemData._ItemName;
            
            _closeButton.onClick.AddListener(Close);
            _uiManager.TryClosePanel<InventoryPanel>();
            m_returnToInventory = readableData.ReturnToIventory;
        }

        public override void Close()
        {
            _closeButton.onClick.RemoveListener(Close);
            if (m_returnToInventory) _uiManager.OpenPanel<InventoryPanel>(new UIPanelParams());
            base.Close();
        }
    }
}