using System.Collections.Generic;

namespace Game.CMS.Runtime
{
    public class CMSTable<T> where T : CMSEntity
    {
        private readonly List<T> _items = new();
        private readonly Dictionary<string, T> _itemsById = new();

        public void Add(T instance)
        {
            _items.Add(instance);
            _itemsById.Add(instance.EntityId, instance);
        }

        public List<T> GetAll()
        {
            return _items;
        }

        public T GetEntityOrDefault(string id)
        {
            return id != null && _itemsById.TryGetValue(id, out T instance) ? instance : default;
        }
    }
}
