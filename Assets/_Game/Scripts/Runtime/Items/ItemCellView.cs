using Game.Runtime.Data.CMSComponents.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Items
{
    public class ItemCellView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;
        [SerializeField] private Image _productionProgressImage;

        public RectTransform ProductionPoint => _productionProgressImage.rectTransform;

        public void Setup(Vector2Int gridPosition, ItemViewComponent config, bool isProductionTile)
        {
            name = $"ItemCell {gridPosition}";

            _rectTransform.SetAsFirstSibling();
            _rectTransform.localPosition = Vector3.zero;
            _rectTransform.localScale = Vector3.one;
            _rectTransform.sizeDelta = Vector2.one * config.CellSize;
            _rectTransform.anchoredPosition = gridPosition * config.CellSize;

            _image.sprite = config.CellSprite;
            _image.color = isProductionTile ? config.ProductionTileColor : config.ItemColor;
            _image.rectTransform.sizeDelta = Vector2.one * config.CellImageSize;
            _image.raycastTarget = false;

            _productionProgressImage.sprite = config.CellSprite;
            _productionProgressImage.color = config.ProductionProgressColor;
            _productionProgressImage.type = Image.Type.Filled;
            _productionProgressImage.fillAmount = 0f;
            _productionProgressImage.rectTransform.sizeDelta = Vector2.one * config.CellImageSize;
            _productionProgressImage.raycastTarget = false;
            _productionProgressImage.gameObject.SetActive(isProductionTile);
        }

        public void SetProductionPercentage(float percentage)
        {
            _productionProgressImage.fillAmount = percentage;
        }
    }
}
