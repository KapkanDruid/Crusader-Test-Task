using Game.CMS.Runtime;

namespace Game.Runtime.Artifacts
{
    public interface IArtifactProductionHandler
    {
        void Handle(CMSEntity artifact, ArtifactProductionContext context);
    }
}
