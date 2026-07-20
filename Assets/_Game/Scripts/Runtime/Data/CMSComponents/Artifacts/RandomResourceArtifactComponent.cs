using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Artifacts
{
    [Serializable]
    public class RandomResourceArtifactComponent : CMSComponent
    {
        [SerializeField] private float _period;
        [SerializeField] private int _minimumItemCount;
        [SerializeField] private int _resourceCount;

        public float Period => _period;
        public int MinimumItemCount => _minimumItemCount;
        public int ResourceCount => _resourceCount;
    }
}
