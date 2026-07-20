using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Data.CMSComponents.Resources;
using Game.Runtime.DragAndDrop;
using Game.Runtime.Items;
using Game.Runtime.Resources;
using Game.Runtime.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Runtime.UI.DropPanels
{
    [Serializable]
    public class DestructionPanel : IItemHolder, IItemPositionHandler
    {
        [SerializeField] private RectTransform _dropPanel;
        [SerializeField] private RectTransform _costGroup;
        [SerializeField] private Image _panelImage;

        private ResourcesModel _resourcesModel;
        private SpawnPanel _spawnPanel;
        private ItemDestructionPanelConfig _config;
        private ResourcesViewConfigComponent _resourcesViewConfig;
        private Color _defaultColor;

        [Inject]
        private void Construct(ResourcesModel resourcesModel, SpawnPanel spawnPanel)
        {
            _resourcesModel = resourcesModel;
            _spawnPanel = spawnPanel;
        }

        public void Setup()
        {
            _defaultColor = _panelImage.color;
            _config = CMSContainer
                .Get(CMSPath.Configs.DestructionPanelConfig)
                .GetComponent<ItemDestructionPanelConfig>();

            _resourcesViewConfig = CMSContainer
                .Get(CMSPath.Configs.ResourcesConfig)
                .GetComponent<ResourcesViewConfigComponent>();

            foreach (var resourceCost in _config.ResourceCosts)
            {
                AddResourceCost(resourceCost);
            }
        }

        private void AddResourceCost(ResourceCost resourceCost)
        {
            var resourceObject = Object.Instantiate(_resourcesViewConfig.ResourceViewPrefab, _costGroup);
            resourceObject.transform.localScale = Vector3.one * _config.ResourceScale;

            var resource = resourceCost.Resource.AsEntity();
            resourceObject.GetComponentInChildren<Image>().sprite = resource.GetComponent<ResourceComponent>().Sprite;
            resourceObject.GetComponentInChildren<TMP_Text>().text = resourceCost.Count.ToString();
        }

        bool IItemHolder.TryPlaceItem(ItemBehavior item)
        {
            if (!_dropPanel.IsTargetInsideRectTransform(item.ItemRoot) || !CanDestroy())
                return false;

            SpendResources();
            _panelImage.color = _defaultColor;
            Object.Destroy(item.gameObject);
            _spawnPanel.SpawnItem();
            return true;
        }

        bool IItemHolder.TryRemoveItem(ItemBehavior item)
        {
            return false;
        }

        void IItemPositionHandler.HandleItemPosition(ItemBehavior item)
        {
            bool canPlace = _dropPanel.IsTargetInsideRectTransform(item.ItemRoot) && CanDestroy();
            _panelImage.color = canPlace ? _config.OnEnterColor : _defaultColor;
        }

        void IItemPositionHandler.ResetItemPosition(ItemBehavior item)
        {
            _panelImage.color = _defaultColor;
        }

        private bool CanDestroy()
        {
            foreach (var resourceCost in _config.ResourceCosts)
            {
                if (!_resourcesModel.HasResource(resourceCost.Resource, resourceCost.Count))
                    return false;
            }

            return true;
        }

        private void SpendResources()
        {
            foreach (var resourceCost in _config.ResourceCosts)
            {
                _resourcesModel.RemoveResource(resourceCost.Resource, resourceCost.Count);
            }
        }
    }
}
