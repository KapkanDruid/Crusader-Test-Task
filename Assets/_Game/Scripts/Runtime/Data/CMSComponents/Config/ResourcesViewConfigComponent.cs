using Game.CMS.Runtime;
using Game.Runtime.Resources;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Config
{
    [Serializable]
    public class ResourcesViewConfigComponent : CMSComponent
    {
        [SerializeField, Range(0, 10)] private float _resourceScale;
        [SerializeField] private ResourceGroupElement _resourceViewPrefab;
        [SerializeField] private Vector2 _resourcePopupOffset;
        [SerializeField, Range(0, 10)] private float _resourcePopupDuration;

        public float ResourceScale => _resourceScale;
        public ResourceGroupElement ResourceViewPrefab => _resourceViewPrefab;
        public Vector2 ResourcePopupOffset => _resourcePopupOffset;
        public float ResourcePopupDuration => _resourcePopupDuration;
    }
}
