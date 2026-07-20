using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Artifacts
{
    [Serializable]
    public class DestroyProducerChanceArtifactComponent : CMSComponent
    {
        [SerializeField] private CMSAsset _resource;
        [SerializeField, Range(0f, 1f)] private float _chance;
        [SerializeField] private bool _spawnReplacement;

        public CMSAsset Resource => _resource;
        public float Chance => _chance;
        public bool SpawnReplacement => _spawnReplacement;
    }
}
