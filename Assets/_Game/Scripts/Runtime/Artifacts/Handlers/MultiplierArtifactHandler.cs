using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Artifacts;

namespace Game.Runtime.Artifacts.Handlers
{
    public class MultiplierArtifactHandler : IArtifactProductionModifierHandler
    {
        public void Handle(CMSEntity artifact, ArtifactProductionContext context)
        {
            if (!artifact.Is<MultiplierArtifactComponent>(out var component))
                return;

            if (component.Resource.AsEntity() != context.Resource)
                return;

            context.Multiplier *= component.Multiplier;
        }
    }
}
