using System.Collections.Generic;
using Game.Player;
using Game.World.Objects;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.UI
{
    public class InventoryPanel : UiPanel
    {
        [Inject] private PlayerInventoryManager _inventoryManager;

        [SerializeField] private Transform _inventoryItemsGrid;
        [SerializeField] private InventoryItemView _inventoryItemView;
        [SerializeField] private TextMeshProUGUI _itemNameTxt;
        [SerializeField] private TextMeshProUGUI _itemDescTxt;

        private Dictionary<InventoryItemView, IObtainable> m_itemDataSet;
        private InventoryItemView _activeView;
        
        public override void Close()
        {
            base.Close();
            ResetInventoryList();
        }

        protected override void Init()
        {
            UpdateInventoryList();
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
            if (m_itemDataSet == null || m_itemDataSet.Count == 0) return;

            foreach (var item in m_itemDataSet)
            {
                var key = item.Key;
                key.gameObject.SetActive(false);
                key.Reset();
                Destroy(key.gameObject);
            }
            
            m_itemDataSet.Clear();
            _itemDescTxt.text = "";
            _itemNameTxt.text = "";
        }
    }
}