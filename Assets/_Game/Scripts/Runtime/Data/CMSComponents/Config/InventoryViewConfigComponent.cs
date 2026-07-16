using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Config
{
    [Serializable]
    public class InventoryViewConfigComponent : CMSComponent
    {
        [SerializeField] private Sprite _cellSprite;
        [SerializeField, Range(0, 500)] private int _cellSize;
        [SerializeField, Range(0, 500)] private float _cellImageSize;

        public Sprite CellSprite => _cellSprite;
        public int CellSize => _cellSize;
        public float CellImageSize => _cellImageSize;
    }
}
