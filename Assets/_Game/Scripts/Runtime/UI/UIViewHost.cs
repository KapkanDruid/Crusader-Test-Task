using Game.Runtime.UI.DropPanels;
using Game.Runtime.UI.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game.Runtime.UI
{
    public class UIViewHost : MonoBehaviour
    {
        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private SpawnPanel _spawnPanel; 
        [SerializeField] private RectTransform _dragArea;

        public RectTransform DragArea => _dragArea;
        public SpawnPanel SpawnPanel => _spawnPanel;
        public InventoryView InventoryView => _inventoryView; 
    }
}