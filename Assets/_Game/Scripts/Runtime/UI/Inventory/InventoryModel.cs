using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.DragAndDrop;
using Game.Runtime.Items;
using Game.Runtime.UI.DropPanels;
using Game.Runtime.Utils;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Runtime.UI.Inventory
{
    public class InventoryModel : IInventoryViewCommands, IItemPositionHandler, IItemHolder, IDisposable
    {
        private ObservableHashSet<Vector2Int> _gridPositions = new();
        private ObservableDictionary<Vector2Int, InventoryTileType> _tileTypes = new();
        private Dictionary<Vector2Int, RectTransform> _gridCells = new();
        private Dictionary<Vector2Int, ItemBehavior> _itemsByGridPosition = new();
        private Dictionary<ItemBehavior, Vector2Int> _itemPositions = new();
        private ObservableHashSet<ItemBehavior> _itemsInInventory = new();
        private ReactiveProperty<(ItemBehavior Item, RectTransform Slot, ItemRotation Rotation)> _placementPreview = new();
        private SpawnPanel _spawnPanel;
        private InventoryTileGeneratorConfigComponent _tileGeneratorConfig;

        public IObservableCollection<Vector2Int> GridPositions => _gridPositions;
        public IReadOnlyObservableDictionary<Vector2Int, InventoryTileType> TileTypes => _tileTypes;
        public IObservableCollection<ItemBehavior> ItemsInInventory => _itemsInInventory;
        public Observable<(ItemBehavior Item, RectTransform Slot, ItemRotation Rotation)> PlacementPreview => _placementPreview;

        public InventoryModel(SpawnPanel spawnPanel)
        {
            _spawnPanel = spawnPanel;
        }

        public void GenerateGrid()
        {
            var configEntity = CMSContainer.Get(CMSPath.Configs.InventoryConfig);
            var gridConfig = configEntity.GetComponent<GridCMSComponent>();
            _tileGeneratorConfig = configEntity.GetComponent<InventoryTileGeneratorConfigComponent>();

            _gridPositions.AddRange(gridConfig.GridPattern);
            GenerateTileTypes();
        }

        public InventoryTileType GetTileType(ItemBehavior item, Vector2Int itemTilePosition)
        {
            Vector2Int gridPosition = _itemPositions[item] + itemTilePosition;
            return _tileTypes[gridPosition];
        }

        private void GenerateTileTypes()
        {
            var gridPositions = _gridPositions.ToList();
            int minX = gridPositions.Min(position => position.x);
            int maxX = gridPositions.Max(position => position.x);
            int minY = gridPositions.Min(position => position.y);
            int maxY = gridPositions.Max(position => position.y);
            var remainingPositions = new List<Vector2Int>();

            foreach (var gridPosition in gridPositions)
            {
                bool isRed = gridPosition.x < minX + _tileGeneratorConfig.RedTileBorderSize
                             || gridPosition.x > maxX - _tileGeneratorConfig.RedTileBorderSize
                             || gridPosition.y < minY + _tileGeneratorConfig.RedTileBorderSize
                             || gridPosition.y > maxY - _tileGeneratorConfig.RedTileBorderSize;

                if (isRed)
                    _tileTypes.Add(gridPosition, InventoryTileType.Red);
                else
                    remainingPositions.Add(gridPosition);
            }

            remainingPositions.Shuffle();
            int yellowTileCount = Mathf.CeilToInt(remainingPositions.Count * _tileGeneratorConfig.YellowTilePercentage);

            for (int i = 0; i < remainingPositions.Count; i++)
            {
                var tileType = i < yellowTileCount
                    ? InventoryTileType.Yellow
                    : InventoryTileType.Green;

                _tileTypes.Add(remainingPositions[i], tileType);
            }
        }

        bool IInventoryViewCommands.RemoveCell(Vector2Int gridPosition)
        {
            return _gridCells.Remove(gridPosition);
        }

        void IInventoryViewCommands.SetCell(Vector2Int gridPosition, RectTransform item)
        {
            _gridCells[gridPosition] = item;
        }

        bool IItemHolder.TryPlaceItem(ItemBehavior item)
        {
            if (!IsInsideInventory(item, out var slot))
                return false;

            if (!CanPlaceInInventory(item, slot, out var itemGridPositions))
                return false;

            PlaceInInventory(item, slot, itemGridPositions);
            return true;
        }

        bool IItemHolder.TryRemoveItem(ItemBehavior item)
        {
            return RemoveFromInventory(item);
        }

        void IItemPositionHandler.HandleItemPosition(ItemBehavior item)
        {
            if (IsInsideInventory(item, out var slot)
                && CanPlaceInInventory(item, slot, out _))
            {
                _placementPreview.Value = (item, slot, item.CurrentRotation);
                return;
            }

            _placementPreview.Value = default;
        }

        void IItemPositionHandler.ResetItemPosition(ItemBehavior item)
        {
            _placementPreview.Value = default;
        }

        private bool IsInsideInventory(ItemBehavior item, out RectTransform slot)
        {
            slot = GetSlot(item);

            if (slot != null)
                return true;

            return false;
        }

        private RectTransform GetSlot(ItemBehavior item)
        {
            foreach (var slot in _gridCells)
            {
                if (slot.Value.IsTargetInsideRectTransform(item.ItemRoot))
                {
                    return slot.Value;
                }
            }

            return null;
        }

        private bool CanPlaceInInventory(ItemBehavior item, RectTransform slot, out List<Vector2Int> itemGridPositions)
        {
            itemGridPositions = null;

            if (!TryGetSlotPosition(slot, out var slotPosition))
                return false;

            var positionsToCheck = item.SlotPositions
                .Select(position => position + slotPosition)
                .Distinct()
                .ToList();

            if (positionsToCheck.Count == 0)
                return false;

            if (positionsToCheck.Any(position => !_gridCells.ContainsKey(position)))
                return false;

            itemGridPositions = positionsToCheck;
            return true;
        }

        private bool TryGetSlotPosition(RectTransform slot, out Vector2Int slotPosition)
        {
            foreach (var gridCell in _gridCells)
            {
                if (gridCell.Value == slot)
                {
                    slotPosition = gridCell.Key;
                    return true;
                }
            }

            slotPosition = default;
            return false;
        }

        private void PlaceInInventory(ItemBehavior item, RectTransform slot, IReadOnlyList<Vector2Int> itemGridPositions)
        {
            var displacedItems = GetDisplacedItems(item, itemGridPositions);

            RemoveFromInventory(item);

            foreach (var displacedItem in displacedItems)
            {
                RemoveFromInventory(displacedItem);
            }

            item.ItemRoot.SetParent(slot.parent, false);
            item.ItemRoot.SetAsLastSibling();
            item.ItemRoot.anchoredPosition = slot.anchoredPosition;

            foreach (var gridPosition in itemGridPositions)
            {
                _itemsByGridPosition[gridPosition] = item;
            }

            TryGetSlotPosition(slot, out var itemPosition);
            _itemPositions[item] = itemPosition;

            if (!_itemsInInventory.Contains(item))
                _itemsInInventory.Add(item);

            _spawnPanel.ReturnItems(displacedItems);
        }

        private List<ItemBehavior> GetDisplacedItems(ItemBehavior placedItem, IReadOnlyList<Vector2Int> targetPositions)
        {
            return targetPositions
                .Where(position => _itemsByGridPosition.ContainsKey(position))
                .Select(position => _itemsByGridPosition[position])
                .Where(storedItem => storedItem != placedItem)
                .Distinct()
                .ToList();
        }

        private bool RemoveFromInventory(ItemBehavior item)
        {
            var occupiedPositions = _itemsByGridPosition
                .Where(pair => pair.Value == item)
                .Select(pair => pair.Key)
                .ToList();

            foreach (var occupiedPosition in occupiedPositions)
            {
                _itemsByGridPosition.Remove(occupiedPosition);
            }

            _itemPositions.Remove(item);
            _itemsInInventory.Remove(item);

            return occupiedPositions.Count > 0;
        }

        public void Dispose()
        {
            _placementPreview.Dispose();
        }
    }
}
