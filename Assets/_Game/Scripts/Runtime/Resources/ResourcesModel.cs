using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Resources;
using Game.Runtime.Utils;
using ObservableCollections;
using System;
using UnityQuickTests;

namespace Game.Runtime.Resources
{
    public class ResourcesModel
    {
        private readonly ObservableDictionary<CMSEntity, int> _resources = new();

        public IReadOnlyObservableDictionary<CMSEntity, int> Resources => _resources;

        public void AddResource(CMSEntity entity, int count)
        {
            ValidateOperation(entity, count);

            if (!entity.Is<ResourceComponent>())
                return;

            if (_resources.TryGetValue(entity, out int currentCount))
                _resources[entity] = checked(currentCount + count);
            else
                _resources.Add(entity, count);
        }

        public bool HasResource(CMSEntity entity, int count)
        {
            ValidateOperation(entity, count);

            if (!entity.Is<ResourceComponent>())
                return false;

            return _resources.TryGetValue(entity, out int currentCount) && currentCount >= count;
        }

        public int GetResourceCount(CMSEntity entity)
        {
            return _resources.TryGetValue(entity, out int count) ? count : 0;
        }

        public void RemoveResource(CMSEntity entity, int count)
        {
            ValidateOperation(entity, count);

            if (!entity.Is<ResourceComponent>())
                return;

            if (!_resources.TryGetValue(entity, out int currentCount))
                return;

            int remainingCount = currentCount - count;

            if (remainingCount > 0)
                _resources[entity] = remainingCount;
            else
                _resources.Remove(entity);
        }

        private static void ValidateOperation(CMSEntity entity, int count)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "Resource count must be greater than zero.");
        }
    }
}
