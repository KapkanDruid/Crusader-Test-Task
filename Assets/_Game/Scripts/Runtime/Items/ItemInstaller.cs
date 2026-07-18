using Game.CMS.Runtime;
using UnityEngine;
using Zenject;

namespace Game.Runtime.Items
{
    public class ItemInstaller : MonoInstaller
    {
        [Inject] private CMSEntity _itemDataEntity;
        [Inject] private RectTransform _dragArea;

        public override void InstallBindings()
        {
            Container.Bind<ItemBehavior>()
                .FromComponentOnRoot()
                .AsSingle();

            Container.Bind<CMSEntity>()
                .FromInstance(_itemDataEntity)
                .AsSingle();

            Container.BindInstance(_dragArea)
                .WhenInjectedInto<ItemBehavior>();

            Container.Bind<ItemTriggerHandler>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<ItemDragHandler>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<ItemView>()
                .AsSingle();
        }
    }
}
