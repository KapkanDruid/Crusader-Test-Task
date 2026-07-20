using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Artifacts;
using Game.Runtime.UI.Inventory;
using UnityEngine;

namespace Game.Runtime.Artifacts.Handlers
{
    public class DestroyProducerChanceArtifactHandler : IArtifactProductionHandler
    {
        private readonly InventoryModel _inventoryModel;

        public DestroyProducerChanceArtifactHandler(InventoryModel inventoryModel)
        {
            _inventoryModel = inventoryModel;
        }

        public void Handle(CMSEntity artifact, ArtifactProductionContext context)
        {
            if (!artifact.Is<DestroyProducerChanceArtifactComponent>(out var component))
                return;

            if (component.Resource.AsEntity() != context.Resource || Random.value > component.Chance)
                return;

            _inventoryModel.DestroyItem(context.Item, component.SpawnReplacement);
        }
    }
}
