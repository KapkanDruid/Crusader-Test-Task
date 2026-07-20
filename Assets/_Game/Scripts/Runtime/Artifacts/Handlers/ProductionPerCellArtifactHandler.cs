using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Artifacts;

namespace Game.Runtime.Artifacts.Handlers
{
    public class ProductionPerCellArtifactHandler : IArtifactProductionModifierHandler
    {
        public void Handle(CMSEntity artifact, ArtifactProductionContext context)
        {
            if (!artifact.Is<ProductionPerCellArtifactComponent>(out var component))
                return;

            if (component.Resource.AsEntity() != context.Resource)
                return;

            context.AdditionalCount += context.Item.SlotPositions.Count * component.CountPerCell;
        }
    }
}
