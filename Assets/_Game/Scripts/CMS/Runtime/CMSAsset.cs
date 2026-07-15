using System.Collections.Generic;
using UnityEngine;

namespace Game.CMS.Runtime
{
    public class CMSAsset : ScriptableObject
    {
        [SerializeField, HideInInspector] private string entityId = string.Empty;
        [SerializeReference] public List<CMSComponent> Components = new();

        public string EntityId => entityId;

        public CMSEntity AsEntity()
        {
            return CMSContainer.Get(entityId);
        }

        public static implicit operator CMSEntity(CMSAsset cmsAsset)
        {
            return cmsAsset.AsEntity();
        }

#if UNITY_EDITOR
        public void PingEntity()
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(this);

            if (path.StartsWith("Assets/_Game/Resources/CMS/") && path.EndsWith(".asset"))
            {
                path = path.Substring("Assets/_Game/Resources/".Length);
                path = path.Substring(0, path.Length - ".asset".Length);
            }

            entityId = path;
            Components.RemoveAll(component => component == null);
        }
#endif
    }
}
