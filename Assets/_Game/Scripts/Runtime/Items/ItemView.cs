using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Data.CMSComponents.Item;
using Game.Runtime.Resources;
using UnityEngine;

namespace Game.Runtime.Items
{
    public class ItemView
    {
        private CMSEntity _dataEntity;
        private ResourcePopupService _resourcePopupService;
        private RectTransform _itemRoot;
        private RectTransform _viewRoot;
        private RectTransform _highlightRoot;
        private CanvasGroup _viewCanvasGroup;
        private ItemViewComponent _configComponent;
        private ItemCellView _productionCell;

        public ItemView(CMSEntity dataEntity, ResourcePopupService resourcePopupService)
        {
            _dataEntity = dataEntity;
            _resourcePopupService = resourcePopupService;
        }

        public void Setup(RectTransform viewRoot,
                          CanvasGroup viewCanvasGroup,
                          RectTransform highlightRoot,
                          CanvasGroup highlightCanvasGroup,
                          RectTransform itemRoot,
                          ItemCellView cellPrefab,
                          Vector2Int productionTilePosition)
        {
            _itemRoot = itemRoot;
            _viewRoot = viewRoot;
            _viewCanvasGroup = viewCanvasGroup;
            _highlightRoot = highlightRoot;
            _configComponent = _dataEntity.GetComponent<ItemViewComponent>();
            var gridConfig = _dataEntity.GetComponent<GridCMSComponent>();

            foreach (var gridPosition in gridConfig.GridPattern)
            {
                bool isProductionTile = gridPosition == productionTilePosition;
                var cell = Object.Instantiate(cellPrefab, _viewRoot);
                cell.Setup(gridPosition, _configComponent, isProductionTile);

                var highlightCell = Object.Instantiate(cellPrefab, _highlightRoot);
                highlightCell.Setup(gridPosition, _configComponent, isProductionTile);

                if (isProductionTile)
                    _productionCell = cell;
            }

            highlightCanvasGroup.alpha = 1f;
            highlightCanvasGroup.blocksRaycasts = false;
            highlightCanvasGroup.interactable = false;
            _highlightRoot.gameObject.SetActive(false);
        }

        public void ShowHighlight(RectTransform inventoryRoot, Vector2 position)
        {
            _highlightRoot.SetParent(inventoryRoot, false);
            _highlightRoot.anchoredPosition = position;
            _highlightRoot.localRotation = _itemRoot.localRotation;
            _highlightRoot.SetAsLastSibling();
            _highlightRoot.gameObject.SetActive(true);
        }

        public void HideHighlight()
        {
            _highlightRoot.gameObject.SetActive(false);
            _highlightRoot.SetParent(_itemRoot, false);
            _highlightRoot.anchoredPosition = _viewRoot.anchoredPosition;
            _highlightRoot.localRotation = Quaternion.identity;
        }

        public void SetDimmed(bool isDimmed)
        {
            _viewCanvasGroup.alpha = isDimmed ? _configComponent.DimmedAlpha : 1f;
        }

        public void SetProductionPercentage(float percentage)
        {
            _productionCell.SetProductionPercentage(percentage);
        }

        public void ProductionComplete(CMSEntity resource, int count)
        {
            _resourcePopupService.Show(_productionCell.ProductionPoint, resource, count);
        }
    }
}
