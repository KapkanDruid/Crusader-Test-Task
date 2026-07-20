using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Config
{
    [Serializable]
    public class InventoryTileGeneratorConfigComponent : CMSComponent
    {
        [SerializeField, Min(0)] private int _redTileBorderSize = 1;
        [SerializeField, Range(0f, 1f)] private float _yellowTilePercentage = 1f / 3f;

        public int RedTileBorderSize => _redTileBorderSize;
        public float YellowTilePercentage => _yellowTilePercentage;
    }
}
