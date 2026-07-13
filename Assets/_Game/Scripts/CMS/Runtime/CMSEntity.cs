using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.CMS.Runtime
{
    public class CMSEntity
    {
        private readonly List<CMSComponent> _components;

        public string EntityId { get; }

        public CMSEntity(string entityId, List<CMSComponent> components = null)
        {
            EntityId = entityId;
            _components = components ?? new List<CMSComponent>();
        }

        public T Define<T>() where T : CMSComponent, new()
        {
            T component = GetComponent<T>();
            if (component != null)
                return component;

            component = new T();
            _components.Add(component);
            return component;
        }

        public bool Is<T>(out T component) where T : CMSComponent, new()
        {
            component = GetComponent<T>();
            return component != null;
        }

        public bool Is<T>() where T : CMSComponent, new()
        {
            return GetComponent<T>() != null;
        }

        public bool Is(Type type)
        {
            return _components.Exists(component => component != null && component.GetType() == type);
        }

        public T GetComponent<T>() where T : CMSComponent, new()
        {
            return _components.Find(component => component is T) as T;
        }

        public List<T> GetComponents<T>() where T : CMSComponent, new()
        {
            return _components.OfType<T>().ToList();
        }

        public List<CMSComponent> GetAll()
        {
            return _components;
        }
    }
}
