using Game.CMS.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Config
{
    [Serializable]
    public sealed class GridCMSComponent : CMSComponent
    {
        [Serializable]
        public sealed class GridData
        {
            [SerializeField] private List<Vector2Int> gridPattern = new();
            [SerializeField] private Vector2Int gridSize;

            public Vector2Int GridSize => gridSize;
            public IReadOnlyList<Vector2Int> GridPattern =>
                gridPattern != null ? gridPattern : Array.Empty<Vector2Int>();

            public void ClearPattern()
            {
                gridPattern ??= new();
                gridPattern.Clear();
            }

            public string GetPositionsString()
            {
                return gridPattern == null ? string.Empty : string.Join(" ", gridPattern);
            }
        }

        [SerializeField] private GridData gridData = new();

        public Vector2Int GridSize => gridData?.GridSize ?? default;
        public IReadOnlyList<Vector2Int> GridPattern => gridData?.GridPattern ?? Array.Empty<Vector2Int>();

        public void ClearPattern()
        {
            gridData ??= new GridData();
            gridData.ClearPattern();
        }

        public string GetPositionsString()
        {
            return gridData?.GetPositionsString() ?? string.Empty;
        }
    }
}
