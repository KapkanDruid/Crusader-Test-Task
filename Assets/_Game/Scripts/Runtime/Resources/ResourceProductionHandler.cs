using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Data.CMSComponents.Item;
using Game.Runtime.Items;
using Game.Runtime.Ticks;
using Game.Runtime.UI.Inventory;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;

namespace Game.Runtime.Resources
{
    public class ResourceProductionHandler : IDisposable
    {
        private class ProductionState
        {
            public ItemBehavior Item;
            public CMSEntity Resource;
            public float Elapsed;
            public float Period;
        }

        private readonly TickService _tickService;
        private readonly InventoryModel _inventoryModel;
        private readonly ResourcesModel _resourcesModel;
        private readonly Dictionary<ItemBehavior, ProductionState> _items = new();
        private readonly List<ProductionState> _tickBuffer = new();
        private readonly CompositeDisposable _disposable = new();

        private ResourceProductionConfigComponent _productionConfig;

        public ResourceProductionHandler(TickService tickService,
                                         InventoryModel inventoryModel,
                                         ResourcesModel resourcesModel)
        {
            _tickService = tickService;
            _inventoryModel = inventoryModel;
            _resourcesModel = resourcesModel;
        }

        public void Setup()
        {
            _productionConfig = CMSContainer
                .Get(CMSPath.Configs.ResourcesConfig)
                .GetComponent<ResourceProductionConfigComponent>();

            _inventoryModel.ItemsInInventory
                .ObserveAdd()
                .Subscribe(eventData => AddItem(eventData.Value))
                .AddTo(_disposable);

            _inventoryModel.ItemsInInventory
                .ObserveRemove()
                .Subscribe(eventData => RemoveItem(eventData.Value))
                .AddTo(_disposable);

            _tickService.OnTick
                .Subscribe(Tick)
                .AddTo(_disposable);

            foreach (var item in _inventoryModel.ItemsInInventory)
            {
                AddItem(item);
            }
        }

        private void AddItem(ItemBehavior item)
        {
            var itemEntity = item.ItemDataEntity;

            if (!itemEntity.Is<ResourceProductionItemComponent>())
                return;

            var productionComponent = itemEntity.GetComponent<ResourceProductionItemComponent>();

            if (productionComponent.Resource == null)
                return;

            _items[item] = new ProductionState
            {
                Item = item,
                Resource = productionComponent.Resource,
                Elapsed = 0f,
                Period = GetProductionPeriod(item),
            };

            item.SetProductionPercentage(0f);
        }

        private void RemoveItem(ItemBehavior item)
        {
            if (!_items.Remove(item))
                return;

            item.SetProductionPercentage(0f);
        }

        private void Tick(float deltaTime)
        {
            _tickBuffer.Clear();
            _tickBuffer.AddRange(_items.Values);

            foreach (var state in _tickBuffer)
            {
                if (!_items.ContainsKey(state.Item))
                    continue;

                if (state.Period <= 0f)
                {
                    state.Item.SetProductionPercentage(0f);
                    continue;
                }

                state.Elapsed += deltaTime;

                while (state.Elapsed >= state.Period)
                {
                    state.Elapsed -= state.Period;
                    Produce(state);

                    if (!_items.ContainsKey(state.Item))
                        break;
                }

                if (_items.ContainsKey(state.Item))
                    state.Item.SetProductionPercentage(state.Elapsed / state.Period);
            }
        }

        private void Produce(ProductionState state)
        {
            int resourceCount = _productionConfig.ResourceCountPerProduction;

            _resourcesModel.AddResource(state.Resource, resourceCount);
            state.Item.ProductionComplete(state.Resource, resourceCount);
        }

        private float GetProductionPeriod(ItemBehavior item)
        {
            var tileType = _inventoryModel.GetTileType(item, item.ProductionTilePosition);
            float productionModifier = GetProductionModifier(tileType);

            if (productionModifier <= 0f)
                return 0f;

            return _productionConfig.ProductionSpeedSeconds / productionModifier;
        }

        private float GetProductionModifier(InventoryTileType tileType)
        {
            return tileType switch
            {
                InventoryTileType.Red => _productionConfig.RedTileProductionModifier,
                InventoryTileType.Yellow => _productionConfig.YellowTileProductionModifier,
                InventoryTileType.Green => _productionConfig.GreenTileProductionModifier,
                _ => throw new ArgumentOutOfRangeException(nameof(tileType), tileType, null),
            };
        }

        public void Dispose()
        {
            _disposable.Dispose();

            foreach (var item in _items.Keys)
            {
                if (item != null)
                    item.SetProductionPercentage(0f);
            }

            _items.Clear();
            _tickBuffer.Clear();
        }
    }
}
