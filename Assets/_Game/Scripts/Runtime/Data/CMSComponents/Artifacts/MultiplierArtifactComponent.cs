using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Artifacts
{
    [Serializable]
    public class MultiplierArtifactComponent : CMSComponent
    {
        [SerializeField] private CMSAsset _resource;
        [SerializeField] private float _multiplier;

        public CMSAsset Resource => _resource;
        public float Multiplier => _multiplier;
    }
}
