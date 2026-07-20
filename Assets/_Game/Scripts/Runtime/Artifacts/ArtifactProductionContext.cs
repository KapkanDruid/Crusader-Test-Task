using Game.CMS.Runtime;
using Game.Runtime.Items;

namespace Game.Runtime.Artifacts
{
    public class ArtifactProductionContext
    {
        public ItemBehavior Item;
        public CMSEntity Resource;
        public int BaseCount;
        public int AdditionalCount;
        public float Multiplier;
        public int Count;

        public ArtifactProductionContext(ItemBehavior item, CMSEntity resource, int baseCount)
        {
            Item = item;
            Resource = resource;
            BaseCount = baseCount;
            Multiplier = 1f;
            Count = baseCount;
        }
    }
}
