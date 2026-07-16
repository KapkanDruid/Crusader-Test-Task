using Game.Runtime.UI;
using Game.Runtime.UI.Inventory;
using UnityEngine;
using Zenject;

namespace Game.Runtime.Bootstrap
{
    public class InventoryInstaller : MonoInstaller
    {
        [SerializeField] private UIViewHost _viewHost;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<InventoryBootstrap>()
                .AsSingle();

            Container.Bind<InventoryModel>()
                .AsSingle();

            Container.Bind<IInventoryViewCommands>()
                .To<InventoryModel>()
                .FromResolve()
                .WhenInjectedInto<InventoryView>();

            Container.Bind<UIViewHost>()
                .FromInstance(_viewHost)
                .AsSingle();
        }
    }
}
