using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Artifacts;
using Game.Runtime.Data.CMSComponents.Resources;
using Game.Runtime.Resources;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Artifacts.Handlers
{
    public class SmallestResourceChanceArtifactHandler : IArtifactProductionHandler
    {
        private readonly ResourcesModel _resourcesModel;
        private readonly ArtifactPopupService _popupService;

        public SmallestResourceChanceArtifactHandler(ResourcesModel resourcesModel, ArtifactPopupService popupService)
        {
            _resourcesModel = resourcesModel;
            _popupService = popupService;
        }

        public void Handle(CMSEntity artifact, ArtifactProductionContext context)
        {
            if (!artifact.Is<SmallestResourceChanceArtifactComponent>(out var component))
                return;

            if (Random.value > component.Chance)
                return;

            var resources = CMSContainer.GetAll<ResourceComponent>();
            var smallestResources = new List<CMSEntity>();
            int smallestCount = int.MaxValue;

            foreach (var resource in resources)
            {
                int count = _resourcesModel.GetResourceCount(resource);

                if (count < smallestCount)
                {
                    smallestCount = count;
                    smallestResources.Clear();
                    smallestResources.Add(resource);
                }
                else if (count == smallestCount)
                {
                    smallestResources.Add(resource);
                }
            }

            if (smallestResources.Count == 0)
                return;

            var selectedResource = smallestResources[Random.Range(0, smallestResources.Count)];
            _resourcesModel.AddResource(selectedResource, component.ResourceCount);
            _popupService.Show(artifact, selectedResource, component.ResourceCount);
        }
    }
}
