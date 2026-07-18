using Game.Runtime.Items;

namespace Game.Runtime.DragAndDrop
{
    public interface IItemHolder
    {
        public bool TryPlaceItem(ItemBehavior item);
        public bool TryRemoveItem(ItemBehavior item);
    }
}
