using Game.Runtime.Items;
using Game.Runtime.Utils;
using System.Collections.Generic;

namespace Game.Runtime.DragAndDrop
{
    public class DragService
    {
        private readonly IItemHolder[] _itemHolders;
        private readonly IItemPositionHandler[] _positionHandlers;

        private IItemHolder _previousHolder;

        public DragService(List<IItemHolder> itemHolders, List<IItemPositionHandler> positionHandlers)
        {
            _itemHolders = itemHolders.ToArray();
            _positionHandlers = positionHandlers.ToArray();
        }
        
        public void StartDrag(ItemBehavior item) 
        {
            for (int i = 0; i < _itemHolders.Length; i++) 
            {
                IItemHolder itemHolder = _itemHolders[i];
                if (itemHolder.TryRemoveItem(item))
                {
                    _previousHolder = itemHolder;
                    break;
                }
            }
        }

        public void Drag(ItemBehavior item) 
        {
            for (int i = 0; i < _positionHandlers.Length; i++)
            {
                IItemPositionHandler positionHandler = _positionHandlers[i];
                positionHandler.HandleItemPosition(item);
            }
        }

        public bool TryPlace(ItemBehavior item)
        {
            bool isPlaced = false;

            for (int i = 0; i < _itemHolders.Length; i++)
            {
                IItemHolder itemHolder = _itemHolders[i];

                if (itemHolder.TryPlaceItem(item))
                {
                    isPlaced = true;
                    break;
                }
            }

            ResetItemPosition(item);
            return isPlaced;
        }

        public void Return(ItemBehavior item)
        {
            if (_previousHolder != null && !_previousHolder.TryPlaceItem(item))
                LogUtil.LogError(this, "Failed to return item!");
        }

        private void ResetItemPosition(ItemBehavior item)
        {
            for (int i = 0; i < _positionHandlers.Length; i++)
            {
                IItemPositionHandler positionHandler = _positionHandlers[i];
                positionHandler.ResetItemPosition(item);
            }
        }
    }
}
