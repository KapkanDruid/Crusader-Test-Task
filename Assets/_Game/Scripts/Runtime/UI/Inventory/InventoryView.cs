using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Items;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
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
        private readonly Dictionary<Vector2Int, RectTransform> _cells = new();
        private readonly Dictionary<Vector2Int, Image> _cellImages = new();
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
            var cellObject = new GameObject($"InventoryCell {gridPosition}", typeof(RectTransform));
            var cell = cellObject.GetComponent<RectTransform>();

            cell.SetParent(_inventoryRoot);
            cell.SetAsFirstSibling();
            cell.localPosition = Vector3.zero;
            cell.localScale = Vector3.one;
            cell.sizeDelta = Vector2.one * _config.CellSize;
            cell.anchoredPosition = gridPosition * _config.CellSize;

            var imageObject = new GameObject($"InventoryCellImage {gridPosition}", typeof(Image));
            var image = imageObject.GetComponent<Image>();

            image.rectTransform.SetParent(cell);
            image.rectTransform.localPosition = Vector3.zero;
            image.rectTransform.localScale = Vector3.one;
            image.rectTransform.anchoredPosition = Vector3.zero;

            image.sprite = _config.CellSprite;
            image.rectTransform.sizeDelta = Vector2.one * _config.CellImageSize;
            image.raycastTarget = false;

            _commands.SetCell(gridPosition, cell);
            _cells[gridPosition] = cell;
            _cellImages[gridPosition] = image;
        }

        private void RemoveCel(Vector2Int gridPosition)
        {
            _commands.RemoveCell(gridPosition);
            Object.Destroy(_cells[gridPosition].gameObject);
            _cells.Remove(gridPosition);
            _cellImages.Remove(gridPosition);
        }

        private void SetCellType(Vector2Int gridPosition, InventoryTileType tileType)
        {
            _cellImages[gridPosition].color = tileType switch
            {
                InventoryTileType.Red => _config.RedTileColor,
                InventoryTileType.Yellow => _config.YellowTileColor,
                InventoryTileType.Green => _config.GreenTileColor,
                _ => throw new ArgumentOutOfRangeException(nameof(tileType), tileType, null),
            };
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
                if (item != null)
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
