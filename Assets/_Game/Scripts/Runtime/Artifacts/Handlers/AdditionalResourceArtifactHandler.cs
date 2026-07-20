using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Artifacts;
using Game.Runtime.Resources;
using UnityEngine;

namespace Game.Runtime.Artifacts.Handlers
{
    public class AdditionalResourceArtifactHandler : IArtifactProductionHandler
    {
        private readonly ResourcesModel _resourcesModel;
        private readonly ArtifactPopupService _popupService;

        public AdditionalResourceArtifactHandler(ResourcesModel resourcesModel, ArtifactPopupService popupService)
        {
            _resourcesModel = resourcesModel;
            _popupService = popupService;
        }

        public void Handle(CMSEntity artifact, ArtifactProductionContext context)
        {
            if (!artifact.Is<AdditionalResourceArtifactComponent>(out var component))
                return;

            if (component.SourceResource.AsEntity() != context.Resource || Random.value > component.Chance)
                return;

            var resource = component.AdditionalResource.AsEntity();
            int count = Mathf.RoundToInt(context.Count * component.CountMultiplier);

            if (count <= 0)
                return;

            _resourcesModel.AddResource(resource, count);
            _popupService.Show(artifact, resource, count);
        }
    }
}
