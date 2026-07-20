using Game.CMS.Runtime;

namespace Game.Runtime.Artifacts
{
    public interface IArtifactProductionModifierHandler
    {
        void Handle(CMSEntity artifact, ArtifactProductionContext context);
    }
}
