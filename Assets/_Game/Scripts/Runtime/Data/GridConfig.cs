using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Data
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "Gameplay/GridConfig")]
    public class GridConfig : ScriptableObject
    {
        [SerializeField] private List<Vector2Int> gridPattern = new();
        [SerializeField] private Vector2Int gridSize;

        public Vector2Int GridSize => gridSize;
        public IReadOnlyList<Vector2Int> GridPattern => gridPattern;

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
}
