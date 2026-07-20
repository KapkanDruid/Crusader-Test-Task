using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Data.CMSComponents.Item;
using Game.Runtime.DragAndDrop;
using Game.Runtime.Items;
using Game.Runtime.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Runtime.UI.DropPanels
{
    [Serializable]
    public class SpawnPanel : IItemHolder, IItemPositionHandler
    {
        [SerializeField] private RectTransform _dropPanel;
        [SerializeField] private RectTransform _spawnArea;
        [SerializeField] private Image _panelImage;

        private ItemBehavior.Factory _itemFactory;
        private ItemSpawnPanelConfig _config;
        private UIViewHost _viewHost;
        private Color _defaultColor;
        private readonly HashSet<ItemBehavior> _storedItems = new();
        private readonly List<Vector2> _spawnPoints = new();

        [Inject]
        private void Construct(ItemBehavior.Factory itemFactory, UIViewHost viewHost)
        {
            _itemFactory = itemFactory;
            _viewHost = viewHost;
        }

        public void SpawnItems()
        {
            _defaultColor = _panelImage.color;

            _config = CMSContainer.Get(CMSPath.Configs.SpawnConfig).GetComponent<ItemSpawnPanelConfig>();
            _spawnPoints.Clear();
            _spawnPoints.AddRange(RectPointGenerator.Generate(_spawnArea, _config.SpawnCount));
            var itemDataList = CMSContainer.GetAll<IsStartItemComponent>();
            itemDataList.Shuffle();

            var itemDataQueue = new Queue<CMSEntity>(itemDataList);

            foreach (var point in _spawnPoints)
            {
                var item = _itemFactory.Create(itemDataQueue.CycledDequeue(), _viewHost.DragArea);
                item.ItemRoot.SetParent(_dropPanel);
                item.ItemRoot.anchoredPosition = point;
                _storedItems.Add(item);
            }
        }

        public void SpawnItem()
        {
            var itemDataList = CMSContainer.GetAll<IsStartItemComponent>();
            var item = _itemFactory.Create(itemDataList.GetRandom(), _viewHost.DragArea);
            _storedItems.Add(item);
            item.ItemRoot.SetParent(_dropPanel);
            item.ItemRoot.anchoredPosition = _spawnPoints.GetRandom();
        }

        public void ReturnItems(IReadOnlyList<ItemBehavior> items)
        {
            if (items.Count == 0)
                return;

            var points = RectPointGenerator.Generate(_spawnArea, items.Count);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                item.ItemRoot.SetParent(_dropPanel, false);
                item.ItemRoot.SetAsLastSibling();
                item.ItemRoot.anchoredPosition = points[i];
                _storedItems.Add(item);
            }

            _panelImage.color = _defaultColor;
        }

        bool IItemHolder.TryPlaceItem(ItemBehavior item)
        {
            if (_dropPanel.IsTargetInsideRectTransform(item.ItemRoot))
            {
                _storedItems.Add(item);
                item.ItemRoot.SetParent(_dropPanel);
                _panelImage.color = _defaultColor;

                return true;
            }

            return false;
        }

        bool IItemHolder.TryRemoveItem(ItemBehavior item)
        {
            if (_storedItems.Contains(item))
            {
                _storedItems.Remove(item);
                return true;
            }

            return false;
        }

        void IItemPositionHandler.HandleItemPosition(ItemBehavior item)
        {
            if (_dropPanel.IsTargetInsideRectTransform(item.ItemRoot))
            {
                _panelImage.color = _config.OnEnterColor;
                return;
            }

            _panelImage.color = _defaultColor;
        }

        void IItemPositionHandler.ResetItemPosition(ItemBehavior item)
        {
            _panelImage.color = _defaultColor;
        }
    }
}
