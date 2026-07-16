using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using ObservableCollections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.UI.Inventory
{
    public class InventoryModel : IInventoryViewCommands
    {
        private ObservableHashSet<Vector2Int> _gridPositions = new();
        private Dictionary<Vector2Int, RectTransform> _gridCells = new();

        public IObservableCollection<Vector2Int> GridPositions => _gridPositions; 

        public void GenerateGrid()
        {
            var configEntity = CMSContainer.Get(CMSPath.Configs.InventoryConfig);
            var gridConfig = configEntity.GetComponent<GridCMSComponent>();

            _gridPositions.AddRange(gridConfig.GridPattern);
        }

        bool IInventoryViewCommands.RemoveCell(Vector2Int gridPosition)
        {
            return _gridCells.Remove(gridPosition);
        }

        void IInventoryViewCommands.SetCell(Vector2Int gridPosition, RectTransform item)
        {
            _gridCells[gridPosition] = item;
        }
    }
}