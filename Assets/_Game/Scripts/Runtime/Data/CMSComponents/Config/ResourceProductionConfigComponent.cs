using Game.CMS.Runtime;
using System;
using UnityEngine;

namespace Game.Runtime.Data.CMSComponents.Config
{
    [Serializable]
    public class ResourceProductionConfigComponent : CMSComponent
    {
        [SerializeField] private float _productionSpeedSeconds;
        [SerializeField] private int _resourceCountPerProduction;
        [SerializeField, Range(0, 1)] private float _redTileProductionModifier;
        [SerializeField, Range(0, 1)] private float _yellowTileProductionModifier;
        [SerializeField, Range(0, 1)] private float _greenTileProductionModifier;

        public float ProductionSpeedSeconds => _productionSpeedSeconds;
        public int ResourceCountPerProduction => _resourceCountPerProduction;
        public float RedTileProductionModifier => _redTileProductionModifier;
        public float YellowTileProductionModifier => _yellowTileProductionModifier;
        public float GreenTileProductionModifier => _greenTileProductionModifier;
    }
}
