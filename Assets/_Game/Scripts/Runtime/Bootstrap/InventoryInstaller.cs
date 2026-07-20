using Game.CMS.Runtime;
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
            InstallItemFactory();
            InstallServices();
            InstallResources();
            InstallTicks();
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
    }
}
