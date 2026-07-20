using Game.CMS.Runtime;
using Game.Runtime.Artifacts.Handlers;
using Game.Runtime.Data.CMSComponents.Artifacts;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Resources;
using Game.Runtime.UI.DropPanels;
using Game.Runtime.Utils;
using ObservableCollections;
using System.Collections.Generic;

namespace Game.Runtime.Artifacts
{
    public class ArtifactsModel
    {
        private readonly ResourcesModel _resourcesModel;
        private readonly SpawnPanel _spawnPanel;
        private readonly List<IArtifactProductionModifierHandler> _modifierHandlers;
        private readonly List<IArtifactProductionHandler> _productionHandlers;
        private readonly List<IArtifactAddedHandler> _addedHandlers;
        private readonly RandomResourceArtifactHandler _randomResourceArtifactHandler;
        private readonly ObservableHashSet<CMSEntity> _artifacts = new();
        private readonly ObservableDictionary<CMSEntity, int> _purchaseCost = new();
        private readonly List<CMSEntity> _availableArtifacts = new();

        private ArtifactsConfigComponent _config;

        public IObservableCollection<CMSEntity> Artifacts => _artifacts;
        public IReadOnlyObservableDictionary<CMSEntity, int> PurchaseCost => _purchaseCost;

        public ArtifactsModel(ResourcesModel resourcesModel,
                              SpawnPanel spawnPanel,
                              List<IArtifactProductionModifierHandler> modifierHandlers,
                              List<IArtifactProductionHandler> productionHandlers,
                              List<IArtifactAddedHandler> addedHandlers,
                              RandomResourceArtifactHandler randomResourceArtifactHandler)
        {
            _resourcesModel = resourcesModel;
            _spawnPanel = spawnPanel;
            _modifierHandlers = modifierHandlers;
            _productionHandlers = productionHandlers;
            _addedHandlers = addedHandlers;
            _randomResourceArtifactHandler = randomResourceArtifactHandler;
        }

        public void Setup()
        {
            _randomResourceArtifactHandler.Setup();

            _config = CMSContainer
                .Get(CMSPath.Configs.ArtifactsConfig)
                .GetComponent<ArtifactsConfigComponent>();

            _availableArtifacts.AddRange(CMSContainer.GetAll<ArtifactComponent>());
            _availableArtifacts.Shuffle();

            foreach (var resourceCost in _config.StartCost)
            {
                _purchaseCost.Add(resourceCost.Resource, resourceCost.Count);
            }
        }

        public ArtifactProductionContext ProcessProduction(ArtifactProductionContext context)
        {
            foreach (var artifact in _artifacts)
            {
                foreach (var handler in _modifierHandlers)
                {
                    handler.Handle(artifact, context);
                }
            }

            context.Count = (int)(context.BaseCount * context.Multiplier) + context.AdditionalCount;

            foreach (var artifact in _artifacts)
            {
                foreach (var handler in _productionHandlers)
                {
                    handler.Handle(artifact, context);
                }
            }

            return context;
        }

        public bool TryPurchase()
        {
            if (!CanPurchase())
                return false;

            SpendResources();

            if (_availableArtifacts.Count == 0)
            {
                _spawnPanel.SpawnItem();
                return true;
            }

            var artifact = _availableArtifacts[0];
            _availableArtifacts.RemoveAt(0);
            _artifacts.Add(artifact);

            foreach (var handler in _addedHandlers)
            {
                handler.Handle(artifact);
            }

            IncreaseCost();
            return true;
        }

        public bool CanPurchase()
        {
            foreach (var resourceCost in _purchaseCost)
            {
                if (!_resourcesModel.HasResource(resourceCost.Key, resourceCost.Value))
                    return false;
            }

            return true;
        }

        private void SpendResources()
        {
            foreach (var resourceCost in _purchaseCost)
            {
                _resourcesModel.RemoveResource(resourceCost.Key, resourceCost.Value);
            }
        }

        private void IncreaseCost()
        {
            var resources = new List<CMSEntity>();

            foreach (var resourceCost in _purchaseCost)
            {
                resources.Add(resourceCost.Key);
            }

            foreach (var resource in resources)
            {
                _purchaseCost[resource] = UnityEngine.Mathf.CeilToInt(
                    _purchaseCost[resource] * (1f + _config.CostIncrease));
            }
        }
    }
}
