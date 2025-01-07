using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game.Player;
using Game.World.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI
{
    public interface IInventoryPanelParams : IUiParams
    {
    }
    
    public class InventoryPanel : UiPanel, IInventoryPanelParams
    {
        [Inject] private readonly PlayerInventoryManager _inventoryManager;
     
        [SerializeField] private Transform _inventoryItemsGrid;
        [SerializeField] private InventoryItemView _inventoryItemView;
        [SerializeField] private TextMeshProUGUI _itemNameTxt;
        [SerializeField] private TextMeshProUGUI _itemDescTxt;
        [Header("Buttons")]
        [SerializeField] private Button _lookButton; 

        private Dictionary<InventoryItemView, IObtainable> m_itemDataSet;
        private InventoryItemView _activeView;
        
        public override void Close(bool handleMouse = true)
        {
            base.Close();
            ResetInventoryList();
            _lookButton.onClick.RemoveListener(OnLookButtonPressed);
        }

        protected override void Init(UIPanelParams data)
        {
            UpdateInventoryList();
            
            _lookButton.onClick.AddListener(OnLookButtonPressed);
        }

        private void OnLookButtonPressed()
        {
            if (_activeView == null) return;
            
            if (GetViewData(_activeView, out var data))
            {
                _uiManager.OpenPanel<InventoryDetailPanel>(new InventoryDetailParams(data as ReadableItemData, true));
            }
        }

        private bool GetViewData(InventoryItemView view, out ObtainableItemData data)
        {
            data = null;
            
            if (m_itemDataSet.ContainsKey(view))
            {
                data = m_itemDataSet[view].Data;
                return true;
            }

            return false;
        }
        
        public void OnInventoryItemSelected(InventoryItemView view)
        {
            if (m_itemDataSet.ContainsKey(view))
            {
                _itemDescTxt.text = m_itemDataSet[view].Data._Definition;
                _itemNameTxt.text = m_itemDataSet[view].Data._ItemName;
            }
            
            foreach (var item in m_itemDataSet)
            {
                item.Key.SetActiveState(item.Key == view);
                _activeView = view;
            }
        }
        
        private void UpdateInventoryList()
        {
            if (m_itemDataSet == null) m_itemDataSet = new Dictionary<InventoryItemView, IObtainable>();
            ResetInventoryList();

           var items = _inventoryManager.GetAllInventoryItems();
            
            if (items == null || items.Count == 0) return;

            foreach (var item in items)
            { 
                var view = Instantiate(_inventoryItemView, _inventoryItemsGrid.transform);
                view.Init(item, this);
                view.gameObject.SetActive(true);
                m_itemDataSet.Add(view, item);
            }
        }
        
        private void ResetInventoryList()
        {
            _itemDescTxt.text = "";
            _itemNameTxt.text = "";
            
            if (m_itemDataSet == null || m_itemDataSet.Count == 0) return;

            foreach (var item in m_itemDataSet)
            {
                var key = item.Key;
                key.gameObject.SetActive(false);
                key.Reset();
                Destroy(key.gameObject);
            }
            
            m_itemDataSet.Clear();
        }
    }
}