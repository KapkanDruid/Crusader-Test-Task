using Game.Runtime.Data.CMSComponents.Config;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.UI.Inventory
{
    public class InventoryCellView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Image _image;

        public RectTransform RectTransform => _rectTransform;

        public void Setup(Vector2Int gridPosition, InventoryViewConfigComponent config)
        {
            name = $"InventoryCell {gridPosition}";

            _rectTransform.SetAsFirstSibling();
            _rectTransform.localPosition = Vector3.zero;
            _rectTransform.localScale = Vector3.one;
            _rectTransform.sizeDelta = Vector2.one * config.CellSize;
            _rectTransform.anchoredPosition = gridPosition * config.CellSize;

            _image.sprite = config.CellSprite;
            _image.rectTransform.sizeDelta = Vector2.one * config.CellImageSize;
            _image.raycastTarget = false;
        }

        public void SetType(InventoryTileType tileType, InventoryViewConfigComponent config)
        {
            _image.color = tileType switch
            {
                InventoryTileType.Red => config.RedTileColor,
                InventoryTileType.Yellow => config.YellowTileColor,
                InventoryTileType.Green => config.GreenTileColor,
                _ => throw new ArgumentOutOfRangeException(nameof(tileType), tileType, null),
            };
        }
    }
}
