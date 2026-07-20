using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Artifacts;
using Game.Runtime.Data.CMSComponents.Resources;
using Game.Runtime.Resources;
using Game.Runtime.Ticks;
using Game.Runtime.UI.Inventory;
using Game.Runtime.Utils;
using R3;
using System;
using System.Collections.Generic;

namespace Game.Runtime.Artifacts.Handlers
{
    public class RandomResourceArtifactHandler : IArtifactAddedHandler, IDisposable
    {
        private class State
        {
            public CMSEntity Artifact;
            public RandomResourceArtifactComponent Component;
            public float Elapsed;
        }

        private readonly TickService _tickService;
        private readonly InventoryModel _inventoryModel;
        private readonly ResourcesModel _resourcesModel;
        private readonly ArtifactPopupService _popupService;
        private readonly List<State> _states = new();
        private readonly CompositeDisposable _disposable = new();

        public RandomResourceArtifactHandler(TickService tickService,
                                             InventoryModel inventoryModel,
                                             ResourcesModel resourcesModel,
                                             ArtifactPopupService popupService)
        {
            _tickService = tickService;
            _inventoryModel = inventoryModel;
            _resourcesModel = resourcesModel;
            _popupService = popupService;
        }

        public void Setup()
        {
            _tickService.OnTick
                .Subscribe(Tick)
                .AddTo(_disposable);
        }

        public void Handle(CMSEntity artifact)
        {
            if (!artifact.Is<RandomResourceArtifactComponent>(out var component))
                return;

            _states.Add(new State
            {
                Artifact = artifact,
                Component = component,
            });
        }

        private void Tick(float deltaTime)
        {
            foreach (var state in _states)
            {
                if (_inventoryModel.ItemsInInventory.Count < state.Component.MinimumItemCount)
                    continue;

                state.Elapsed += deltaTime;

                while (state.Elapsed >= state.Component.Period)
                {
                    state.Elapsed -= state.Component.Period;
                    Produce(state);
                }
            }
        }

        private void Produce(State state)
        {
            var resources = CMSContainer.GetAll<ResourceComponent>();

            if (resources.Count == 0)
                return;

            var resource = resources.GetRandom();
            _resourcesModel.AddResource(resource, state.Component.ResourceCount);
            _popupService.Show(state.Artifact, resource, state.Component.ResourceCount);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
