using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Data.CMSComponents.Item;
using Game.Runtime.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Runtime.Items
{
    public class ItemBehavior : MonoBehaviour
    {
        [SerializeField] private RectTransform _itemRoot;
        [SerializeField] private RectTransform _triggerRoot;
        [SerializeField] private Image _parentTrigger;
        [SerializeField] private RectTransform _viewRoot;
        [SerializeField] private CanvasGroup _viewCanvasGroup;
        [SerializeField] private RectTransform _highlightRoot;
        [SerializeField] private CanvasGroup _highlightCanvasGroup;
        [SerializeField] private ItemCellView _cellPrefab;
        [SerializeField] private ItemTriggerCellView _triggerCellPrefab;

        private Vector2Int _productionTilePosition;
        private RectTransform _dragArea;
        private ItemTriggerHandler _itemTriggerHandler;
        private ItemDragHandler _itemDragHandler;
        private ItemView _itemView;
        private List<Vector2Int> _initialSlotPositions = new();

        public CMSEntity ItemDataEntity { get; private set; }
        public RectTransform ItemRoot => _itemRoot;
        public ItemRotation CurrentRotation => _itemDragHandler.CurrentRotation;
        public Vector2Int ProductionTilePosition
        {
            get
            {
                return _itemDragHandler.CurrentRotation switch
                {
                    ItemRotation.Up => _productionTilePosition,
                    ItemRotation.Right => new Vector2Int(_productionTilePosition.y, -_productionTilePosition.x),
                    ItemRotation.Down => new Vector2Int(-_productionTilePosition.x, -_productionTilePosition.y),
                    ItemRotation.Left => new Vector2Int(-_productionTilePosition.y, _productionTilePosition.x),
                    _ => throw new ArgumentOutOfRangeException(nameof(_itemDragHandler.CurrentRotation), _itemDragHandler.CurrentRotation, null)
                };
            }
        }
        public List<Vector2Int> SlotPositions
        {
            get
            {
                return _initialSlotPositions
                    .Select(slot => _itemDragHandler.CurrentRotation switch
                    {
                        ItemRotation.Up => slot,
                        ItemRotation.Right => new Vector2Int(slot.y, -slot.x),
                        ItemRotation.Down => new Vector2Int(-slot.x, -slot.y),
                        ItemRotation.Left => new Vector2Int(-slot.y, slot.x),
                        _ => throw new ArgumentOutOfRangeException(nameof(_itemDragHandler.CurrentRotation), _itemDragHandler.CurrentRotation, null)
                    })
                    .ToList();
            }
        }

        public class Factory : PlaceholderFactory<CMSEntity, RectTransform, ItemBehavior> 
        {
            public override ItemBehavior Create(CMSEntity dataEntity, RectTransform dragArea)
            {
                if (!dataEntity.Is<GridCMSComponent>())
                    throw new ArgumentException($"{dataEntity.EntityId} does not contain {nameof(GridCMSComponent)}");

                if (!dataEntity.Is<ItemViewComponent>())
                    throw new ArgumentException($"{dataEntity.EntityId} does not contain {nameof(ItemViewComponent)}");

                var item = base.Create(dataEntity, dragArea);
                item.SetupItem();
                return item;
            }
        }

        [Inject]
        private void Construct(CMSEntity dataEntity, ItemTriggerHandler itemTriggerHandler, RectTransform rectTransform, ItemDragHandler itemDragHandler, ItemView itemView)
        {
            ItemDataEntity = dataEntity;
            _itemTriggerHandler = itemTriggerHandler;
            _dragArea = rectTransform;
            _itemDragHandler = itemDragHandler;
            _itemView = itemView;
        }

        public void SetupItem()
        {
            SetupTransform();
            _itemTriggerHandler.SetupTrigger(_triggerRoot, _parentTrigger, _triggerCellPrefab);
            _itemDragHandler.Setup(_dragArea, _itemRoot);
            _initialSlotPositions.AddRange(ItemDataEntity.GetComponent<GridCMSComponent>().GridPattern);
            _productionTilePosition = _initialSlotPositions.GetRandom();
            _itemView.Setup(_viewRoot,
                            _viewCanvasGroup,
                            _highlightRoot,
                            _highlightCanvasGroup,
                            _itemRoot,
                            _cellPrefab,
                            _productionTilePosition);

            _itemDragHandler.IsDraggable = true;
        }

        public void ShowInventoryHighlight(RectTransform inventoryRoot, Vector2 position)
        {
            _itemView.ShowHighlight(inventoryRoot, position);
        }

        public void HideInventoryHighlight()
        {
            _itemView.HideHighlight();
        }

        public void SetDimmed(bool isDimmed)
        {
            _itemView.SetDimmed(isDimmed);
        }

        public void SetProductionPercentage(float percentage)
        {
            _itemView.SetProductionPercentage(percentage);
        }

        public void ProductionComplete(CMSEntity resource, int count)
        {
            _itemView.ProductionComplete(resource, count);
        }

        private void SetupTransform()
        {
            _itemRoot.SetParent(_dragArea);
            _itemRoot.SetAsLastSibling();
            _itemRoot.localPosition = Vector3.zero;
            _itemRoot.localScale = Vector3.one;
        }
    }
}
