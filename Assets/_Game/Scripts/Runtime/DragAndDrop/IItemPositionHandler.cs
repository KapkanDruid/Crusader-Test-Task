using Game.Runtime.Items;

namespace Game.Runtime.DragAndDrop
{
    public interface IItemPositionHandler
    {
        public void HandleItemPosition(ItemBehavior item);
        public void ResetItemPosition(ItemBehavior item);
    }
}
