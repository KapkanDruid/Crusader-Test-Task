using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Item
{
    [Serializable]
    public class ItemViewComponent : CMSComponent
    {
        [SerializeField] private Sprite _cellSprite;
        [SerializeField] private Color _itemColor;
        [SerializeField] private Color _productionTileColor;
        [SerializeField] private Color _productionProgressColor;
        [SerializeField, Range(0, 500)] private int _cellSize;
        [SerializeField, Range(0, 500)] private float _cellImageSize;
        [SerializeField, Range(0f, 1f)] private float _dimmedAlpha = 0.5f;

        public Sprite CellSprite => _cellSprite;
        public Color ItemColor => _itemColor;
        public Color ProductionTileColor => _productionTileColor;
        public Color ProductionProgressColor => _productionProgressColor;
        public int CellSize => _cellSize;
        public float CellImageSize => _cellImageSize;
        public float DimmedAlpha => _dimmedAlpha;
    }
}
