using Game.CMS.Runtime;
using Game.Runtime.Artifacts;
using Game.Runtime.Artifacts.Handlers;
using Game.Runtime.DragAndDrop;
using Game.Runtime.Input;
using Game.Runtime.Items;
using Game.Runtime.Resources;
using Game.Runtime.Ticks;
using Game.Runtime.UI;
using Game.Runtime.UI.DropPanels;
using Game.Runtime.UI.Inventory;
using System;
using UnityEngine;
using Zenject;

namespace Game.Runtime.Bootstrap
{
    public class InventoryInstaller : MonoInstaller
    {
        [SerializeField] private UIViewHost _viewHost;
        [SerializeField] private GameObject _itemPrefab;

        public override void InstallBindings()
        {
            InstallBootstrap();
            InstallInventory();
            InstallSpawnPanel();
            InstallDestructionPanel();
            InstallItemFactory();
            InstallServices();
            InstallResources();
            InstallTicks();
            InstallSpeedButtons();
            InstallArtifacts();
        }

        private void InstallBootstrap()
        {
            Container.BindInterfacesTo<InventoryBootstrap>()
                .AsSingle();
        }

        private void InstallInventory()
        {
            Container.Bind(
                    typeof(InventoryModel),
                    typeof(IItemPositionHandler),
                    typeof(IItemHolder),
                    typeof(IDisposable))
                .To<InventoryModel>()
                .AsSingle();

            Container.Bind<IInventoryViewCommands>()
                .To<InventoryModel>()
                .FromResolve()
                .WhenInjectedInto<InventoryView>();

            Container.BindInterfacesAndSelfTo<InventoryView>()
                .FromInstance(_viewHost.InventoryView)
                .AsSingle();

            Container.QueueForInject(_viewHost.InventoryView);
        }

        private void InstallSpawnPanel()
        {
            Container.BindInterfacesAndSelfTo<SpawnPanel>()
                .FromInstance(_viewHost.SpawnPanel)
                .AsSingle();

            Container.QueueForInject(_viewHost.SpawnPanel);
        }

        private void InstallDestructionPanel()
        {
            Container.BindInterfacesAndSelfTo<DestructionPanel>()
                .FromInstance(_viewHost.DestructionPanel)
                .AsSingle();

            Container.QueueForInject(_viewHost.DestructionPanel);
        }

        private void InstallItemFactory()
        {
            Container.BindFactory<CMSEntity, RectTransform, ItemBehavior, ItemBehavior.Factory>()
                .FromSubContainerResolve()
                .ByNewContextPrefab<ItemInstaller>(_itemPrefab);
        }

        private void InstallServices()
        {
            Container.BindInterfacesAndSelfTo<InputService>()
                .AsSingle();

            Container.Bind<DragService>()
                .AsSingle();

            Container.Bind<UIViewHost>()
                .FromInstance(_viewHost)
                .AsSingle();
        }

        private void InstallResources()
        {
            Container.Bind<ResourcesModel>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<ResourceProductionHandler>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<ResourcePopupService>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<ResourcesView>()
                .FromInstance(_viewHost.ResourcesView)
                .AsSingle();

            Container.QueueForInject(_viewHost.ResourcesView);
        }

        private void InstallTicks()
        {
            Container.BindInterfacesAndSelfTo<TickService>()
                .AsSingle();
        }

        private void InstallSpeedButtons()
        {
            Container.BindInterfacesAndSelfTo<SpeedButtonsView>()
                .FromInstance(_viewHost.SpeedButtonsView)
                .AsSingle();

            Container.QueueForInject(_viewHost.SpeedButtonsView);
        }

        private void InstallArtifacts()
        {
            Container.Bind<ArtifactsModel>()
                .AsSingle();

            Container.Bind<ArtifactPopupService>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<ProductionPerCellArtifactHandler>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<MultiplierArtifactHandler>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<AdditionalResourceArtifactHandler>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<SmallestResourceChanceArtifactHandler>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<DestroyProducerChanceArtifactHandler>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<RandomResourceArtifactHandler>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<ArtifactsView>()
                .FromInstance(_viewHost.ArtifactsView)
                .AsSingle();

            Container.QueueForInject(_viewHost.ArtifactsView);
        }
    }
}
