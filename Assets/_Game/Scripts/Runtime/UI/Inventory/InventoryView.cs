using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Items;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Runtime.UI.Inventory
{
    [Serializable]
    public class InventoryView : IDisposable
    {
        [SerializeField] private RectTransform _inventoryRoot;

        private InventoryModel _model;
        private IInventoryViewCommands _commands;

        private InventoryViewConfigComponent _config;
        private readonly CompositeDisposable _disposable = new();
        private readonly Dictionary<Vector2Int, InventoryCellView> _cells = new();
        private readonly HashSet<ItemBehavior> _dimmedItems = new();
        private ItemBehavior _highlightedItem;

        [Inject]
        private void Construct(InventoryModel model, IInventoryViewCommands commands)
        {
            _model = model;
            _commands = commands;
        }

        public void Setup()
        {
            _config = CMSContainer.Get(CMSPath.Configs.InventoryConfig).GetComponent<InventoryViewConfigComponent>();

            _model.GridPositions
                .ObserveAdd()
                .Select(eventData => eventData.Value)
                .Subscribe(AddCell)
                .AddTo(_disposable);

            _model.GridPositions
                .ObserveRemove()
                .Select(eventData => eventData.Value)
                .Subscribe(RemoveCel)
                .AddTo(_disposable);

            _model.TileTypes
                .ObserveAdd()
                .Subscribe(eventData => SetCellType(eventData.Value.Key, eventData.Value.Value))
                .AddTo(_disposable);

            _model.PlacementPreview
                .Subscribe(HandlePlacementPreview)
                .AddTo(_disposable);
        }

        private void AddCell(Vector2Int gridPosition)
        {
            var cell = Object.Instantiate(_config.CellPrefab, _inventoryRoot);
            cell.Setup(gridPosition, _config);

            _commands.SetCell(gridPosition, cell.RectTransform);
            _cells[gridPosition] = cell;
        }

        private void RemoveCel(Vector2Int gridPosition)
        {
            _commands.RemoveCell(gridPosition);
            Object.Destroy(_cells[gridPosition].gameObject);
            _cells.Remove(gridPosition);
        }

        private void SetCellType(Vector2Int gridPosition, InventoryTileType tileType)
        {
            _cells[gridPosition].SetType(tileType, _config);
        }

        private void HandlePlacementPreview((ItemBehavior Item, RectTransform Slot, ItemRotation Rotation) preview)
        {
            if (preview.Item == null)
            {
                HidePlacementPreview();
                return;
            }

            if (_highlightedItem != null && _highlightedItem != preview.Item)
                HidePlacementPreview();

            if (_highlightedItem == null)
            {
                foreach (var item in _model.ItemsInInventory)
                {
                    item.SetDimmed(true);
                    _dimmedItems.Add(item);
                }
            }

            preview.Item.ShowInventoryHighlight(_inventoryRoot, preview.Slot.anchoredPosition);
            _highlightedItem = preview.Item;
        }

        private void HidePlacementPreview()
        {
            if (_highlightedItem != null)
                _highlightedItem.HideInventoryHighlight();

            foreach (var item in _dimmedItems)
            {
                item.SetDimmed(false);
            }

            _dimmedItems.Clear();
            _highlightedItem = null;
        }

        public void Dispose()
        {
            HidePlacementPreview();
            _disposable.Dispose();
        }
    }
}
