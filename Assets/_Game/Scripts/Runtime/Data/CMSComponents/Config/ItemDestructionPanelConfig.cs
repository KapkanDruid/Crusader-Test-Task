using Game.CMS.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Config
{
    [Serializable]
    public class ResourceCost
    {
        [SerializeField] private CMSAsset _resource;
        [SerializeField] private int _count;

        public CMSAsset Resource => _resource;
        public int Count => _count;
    }

    [Serializable]
    public class ItemDestructionPanelConfig : CMSComponent
    {
        [SerializeField] private List<ResourceCost> _resourceCosts = new();
        [SerializeField] private Color _onEnterColor;
        [SerializeField, Range(0, 10)] private float _resourceScale;

        public IReadOnlyList<ResourceCost> ResourceCosts => _resourceCosts;
        public Color OnEnterColor => _onEnterColor;
        public float ResourceScale => _resourceScale; 
    }
}
