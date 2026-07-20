using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Data.CMSComponents.Resources;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Runtime.Resources
{
    [Serializable]
    public class ResourcesView : IDisposable
    {
        [SerializeField] private RectTransform _resourcesGroup;

        private ResourcesModel _model;
        private ResourcesViewConfigComponent _viewConfig;

        private readonly Dictionary<CMSEntity, (Image image, TMP_Text textComponent)> _resources = new();
        private readonly CompositeDisposable _disposable = new();

        [Inject]
        private void Construct(ResourcesModel model)
        {
            _model = model;
        }

        public void Setup()
        {
            _viewConfig = CMSContainer
                .Get(CMSPath.Configs.ResourcesConfig)
                .GetComponent<ResourcesViewConfigComponent>();

            _model.Resources
                .ObserveAdd()
                .Subscribe(eventData => AddResource(eventData.Value.Key, eventData.Value.Value))
                .AddTo(_disposable);

            _model.Resources
                .ObserveRemove()
                .Subscribe(eventData => RemoveResource(eventData.Value.Key))
                .AddTo(_disposable);

            _model.Resources
                .ObserveDictionaryReplace()
                .Subscribe(eventData => ChangeCount(eventData.Key, eventData.NewValue))
                .AddTo(_disposable);
        }

        private void AddResource(CMSEntity entity, int count)
        {
            var resourceObject = Object.Instantiate(_viewConfig.ResourceViewPrefab, _resourcesGroup);
            resourceObject.transform.localScale = Vector3.one * _viewConfig.ResourceScale;

            var resourceImage = resourceObject.GetComponentInChildren<Image>();
            resourceImage.sprite = entity.GetComponent<ResourceComponent>().Sprite;
            
            var resourceText = resourceObject.GetComponentInChildren<TMP_Text>();
            resourceText.text = count.ToString();
            _resources.Add(entity, (resourceImage, resourceText));
        }

        private void RemoveResource(CMSEntity entity)
        {
            if (!_resources.TryGetValue(entity, out var resourceView))
                return;

            _resources.Remove(entity);
            Object.Destroy(resourceView.image.gameObject);
        }

        private void ChangeCount(CMSEntity entity, int count)
        {
            if (entity == null)
                return;

            if (_resources.TryGetValue(entity, out var value))
                value.textComponent.text = count.ToString();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
