namespace Game.CMS.Runtime.Components.Commons
{
    public class CMSPrefabComponent : CMSComponent
    {
        public CMSPrefab Prefab;
        public CMSEntity Entity => Prefab;
    }
}
