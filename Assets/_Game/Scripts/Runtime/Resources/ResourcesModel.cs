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

        [QuickTestHotkey(UnityEngine.KeyCode.A)]
        private void AddOne()
        {
            var random = CMSContainer.GetAll<ResourceComponent>().GetRandom();
            AddResource(random, 1);

            LogUtil.Log("AddOne");
        }

        [QuickTestHotkey(UnityEngine.KeyCode.S)]
        private void AddTwo()
        {
            var random = CMSContainer.GetAll<ResourceComponent>().GetRandom();
            AddResource(random, 2);
        }

        [QuickTestHotkey(UnityEngine.KeyCode.R)]
        private void RemoveOne()
        {
            var random = CMSContainer.GetAll<ResourceComponent>().GetRandom();
            RemoveResource(random, 1);
        }
    }
}
