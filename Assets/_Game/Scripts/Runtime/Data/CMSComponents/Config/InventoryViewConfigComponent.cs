using Game.CMS.Runtime;
using Game.Runtime.UI.Inventory;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Config
{
    [Serializable]
    public class InventoryViewConfigComponent : CMSComponent
    {
        [SerializeField] private Sprite _cellSprite;
        [SerializeField] private Color _redTileColor;
        [SerializeField] private Color _yellowTileColor;
        [SerializeField] private Color _greenTileColor;
        [SerializeField, Range(0, 500)] private int _cellSize;
        [SerializeField, Range(0, 500)] private float _cellImageSize;
        [SerializeField] private InventoryCellView _cellPrefab;

        public Sprite CellSprite => _cellSprite;
        public Color RedTileColor => _redTileColor;
        public Color YellowTileColor => _yellowTileColor;
        public Color GreenTileColor => _greenTileColor;
        public int CellSize => _cellSize;
        public float CellImageSize => _cellImageSize;
        public InventoryCellView CellPrefab => _cellPrefab;
    }
}
