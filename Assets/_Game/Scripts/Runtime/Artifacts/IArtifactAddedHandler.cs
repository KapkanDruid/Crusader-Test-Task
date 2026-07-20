using Game.CMS.Runtime;

namespace Game.Runtime.Artifacts
{
    public interface IArtifactAddedHandler
    {
        void Handle(CMSEntity artifact);
    }
}
