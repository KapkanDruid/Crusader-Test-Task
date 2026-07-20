using Game.CMS.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Config
{
    [Serializable]
    public class ArtifactsConfigComponent : CMSComponent
    {
        [SerializeField] private List<ResourceCost> _startCost = new();
        [SerializeField] private float _costIncrease;
        [SerializeField] private GameObject _artifactViewPrefab;
        [SerializeField, Range(0f, 10f)] private float _artifactScale;
        [SerializeField, Range(0f, 10f)] private float _resourceScale;

        public IReadOnlyList<ResourceCost> StartCost => _startCost;
        public float CostIncrease => _costIncrease;
        public GameObject ArtifactViewPrefab => _artifactViewPrefab;
        public float ArtifactScale => _artifactScale;
        public float ResourceScale => _resourceScale;
    }
}
