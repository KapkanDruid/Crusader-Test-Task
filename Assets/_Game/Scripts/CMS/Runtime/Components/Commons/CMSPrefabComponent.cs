using System;

namespace Game.CMS.Runtime.Components.Commons
{
    [Serializable]
    public class CMSPrefabComponent : CMSComponent
    {
        public CMSPrefab Prefab;
        public CMSEntity Entity => Prefab;
    }
}
