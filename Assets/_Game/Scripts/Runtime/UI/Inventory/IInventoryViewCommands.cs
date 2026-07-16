using UnityEngine;

namespace Game.Runtime.UI.Inventory
{
    public interface IInventoryViewCommands
    {
        public void SetCell(Vector2Int gridPosition, RectTransform item);
        public bool RemoveCell(Vector2Int gridPosition);
    }
}