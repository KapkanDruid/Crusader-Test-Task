using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Artifacts
{
    [Serializable]
    public class AdditionalResourceArtifactComponent : CMSComponent
    {
        [SerializeField] private CMSAsset _sourceResource;
        [SerializeField] private CMSAsset _additionalResource;
        [SerializeField, Range(0f, 1f)] private float _chance;
        [SerializeField] private float _countMultiplier;

        public CMSAsset SourceResource => _sourceResource;
        public CMSAsset AdditionalResource => _additionalResource;
        public float Chance => _chance;
        public float CountMultiplier => _countMultiplier;
    }
}
