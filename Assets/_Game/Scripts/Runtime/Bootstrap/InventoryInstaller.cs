using Game.CMS.Runtime;
using Game.Runtime.DragAndDrop;
using Game.Runtime.Input;
using Game.Runtime.Items;
using Game.Runtime.UI;
using Game.Runtime.UI.DropPanels;
using Game.Runtime.UI.Inventory;
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
        }

        private void InstallBootstrap()
        {
            Container.BindInterfacesTo<InventoryBootstrap>()
                .AsSingle();
        }

        private void InstallInventory()
        {
            Container.BindInterfacesAndSelfTo<InventoryModel>()
                .AsSingle();

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
    }
}
