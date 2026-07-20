using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Artifacts
{
    [Serializable]
    public class SmallestResourceChanceArtifactComponent : CMSComponent
    {
        [SerializeField, Range(0f, 1f)] private float _chance;
        [SerializeField] private int _resourceCount;

        public float Chance => _chance;
        public int ResourceCount => _resourceCount;
    }
}
