using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Item
{
    [Serializable]
    public class ResourceProductionItemComponent : CMSComponent
    {
        [SerializeField] private CMSAsset _resource;
        public CMSAsset Resource => _resource; 
    }
}
