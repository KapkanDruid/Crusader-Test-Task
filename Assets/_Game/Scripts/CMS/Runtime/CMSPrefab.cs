using System.Collections.Generic;
using UnityEngine;

namespace Game.CMS.Runtime
{
    public class CMSPrefab : MonoBehaviour
    {
        [SerializeField, HideInInspector] private string entityId = string.Empty;
        [SerializeReference] public List<CMSComponent> Components = new();

        public string EntityId => entityId;

        public CMSEntity AsEntity()
        {
            return CMSContainer.Get(entityId);
        }

        public static implicit operator CMSEntity(CMSPrefab cmsPrefab)
        {
            return cmsPrefab.AsEntity();
        }

#if UNITY_EDITOR
        public void PingEntity()
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(gameObject);

            if (path.StartsWith("Assets/_Game/Resources/CMS/") && path.EndsWith(".prefab"))
            {
                path = path.Substring("Assets/_Game/Resources/".Length);
                path = path.Substring(0, path.Length - ".prefab".Length);
            }

            entityId = path;
            Components.RemoveAll(component => component == null);
        }
#endif
    }
}
