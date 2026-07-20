using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Data.CMSComponents.Item;
using Game.Runtime.Resources;
using UnityEngine;
using UnityEngine.UI;

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
        private Image _productionProgressImage;

        public ItemView(CMSEntity dataEntity, ResourcePopupService resourcePopupService)
        {
            _dataEntity = dataEntity;
            _resourcePopupService = resourcePopupService;
        }

        public void Setup(RectTransform viewRoot, RectTransform itemRoot, Vector2Int productionTilePosition)
        {
            _itemRoot = itemRoot;
            _viewRoot = viewRoot;
            _configComponent = _dataEntity.GetComponent<ItemViewComponent>();
            var gridConfig = _dataEntity.GetComponent<GridCMSComponent>();

            foreach (var gridPosition in gridConfig.GridPattern)
            {
                AddCell(gridPosition, gridPosition == productionTilePosition);
            }

            _viewCanvasGroup = _viewRoot.gameObject.AddComponent<CanvasGroup>();
            _highlightRoot = Object.Instantiate(_viewRoot, _itemRoot);
            _highlightRoot.name = $"{_viewRoot.name} Highlight";

            var highlightCanvasGroup = _highlightRoot.GetComponent<CanvasGroup>();
            highlightCanvasGroup.alpha = 1f;
            highlightCanvasGroup.blocksRaycasts = false;
            highlightCanvasGroup.interactable = false;

            foreach (var image in _highlightRoot.GetComponentsInChildren<Image>(true))
            {
                image.raycastTarget = false;
            }

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
            _productionProgressImage.fillAmount = percentage;
        }

        public void ProductionComplete(CMSEntity resource, int count)
        {
            _resourcePopupService.Show(_productionProgressImage.rectTransform, resource, count);
        }

        private void AddCell(Vector2Int gridPosition, bool isProductionTile)
        {
            var cellObject = new GameObject($"InventoryCell {gridPosition}", typeof(RectTransform));
            var cell = cellObject.GetComponent<RectTransform>();

            cell.SetParent(_viewRoot);
            cell.SetAsFirstSibling();
            cell.localPosition = Vector3.zero;
            cell.localScale = Vector3.one;
            cell.sizeDelta = Vector2.one * _configComponent.CellSize;
            cell.anchoredPosition = gridPosition * _configComponent.CellSize;

            var imageObject = new GameObject($"InventoryCellImage {gridPosition}", typeof(Image));
            var image = imageObject.GetComponent<Image>();

            image.rectTransform.SetParent(cell);
            image.rectTransform.localPosition = Vector3.zero;
            image.rectTransform.localScale = Vector3.one;
            image.rectTransform.anchoredPosition = Vector3.zero;

            image.sprite = _configComponent.CellSprite;
            image.color = isProductionTile ? _configComponent.ProductionTileColor : _configComponent.ItemColor;
            image.rectTransform.sizeDelta = Vector2.one * _configComponent.CellImageSize;
            image.raycastTarget = false;

            if (!isProductionTile)
                return;

            var productionImageObject = new GameObject($"InventoryProductionCellImage {gridPosition}", typeof(Image));
            _productionProgressImage = productionImageObject.GetComponent<Image>();

            _productionProgressImage.rectTransform.SetParent(cell);
            _productionProgressImage.rectTransform.localPosition = Vector3.zero;
            _productionProgressImage.rectTransform.localScale = Vector3.one;
            _productionProgressImage.rectTransform.anchoredPosition = Vector3.zero;

            _productionProgressImage.sprite = _configComponent.CellSprite;
            _productionProgressImage.color = _configComponent.ProductionProgressColor;
            _productionProgressImage.type = Image.Type.Filled;
            _productionProgressImage.fillAmount = 0f;
            _productionProgressImage.rectTransform.sizeDelta = Vector2.one * _configComponent.CellImageSize;
            _productionProgressImage.raycastTarget = false;
        }
    }
}
