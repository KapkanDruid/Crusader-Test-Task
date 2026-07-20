using Game.Runtime.Resources;
using Game.Runtime.UI.DropPanels;
using Game.Runtime.UI.Inventory;
using UnityEngine;

namespace Game.Runtime.UI
{
    public class UIViewHost : MonoBehaviour
    {
        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private ResourcesView _resourcesView;
        [SerializeField] private SpawnPanel _spawnPanel; 
        [SerializeField] private RectTransform _dragArea;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _resourcePopupRoot;

        public RectTransform DragArea => _dragArea;
        public RectTransform ResourcePopupRoot => _resourcePopupRoot;
        public Canvas Canvas => _canvas;
        public SpawnPanel SpawnPanel => _spawnPanel;
        public InventoryView InventoryView => _inventoryView;
        public ResourcesView ResourcesView => _resourcesView;
    }
}
