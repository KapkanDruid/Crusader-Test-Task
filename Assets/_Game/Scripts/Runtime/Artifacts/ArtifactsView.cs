using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Artifacts;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Data.CMSComponents.Resources;
using Game.Runtime.Resources;
using Game.Runtime.UI;
using ObservableCollections;
using R3;
using R3.Triggers;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Runtime.Artifacts
{
    [Serializable]
    public class ArtifactsView : IDisposable
    {
        [SerializeField] private RectTransform _artifactsGroup;
        [SerializeField] private RectTransform _costGroup;
        [SerializeField] private Button _purchaseButton;
        [SerializeField] private RectTransform _descriptionPanel;
        [SerializeField] private TMP_Text _artifactName;
        [SerializeField] private TMP_Text _artifactDescription;

        private readonly Dictionary<CMSEntity, ResourceGroupElement> _costViews = new();
        private readonly CompositeDisposable _disposable = new();

        private ArtifactsModel _model;
        private ResourcesModel _resourcesModel;
        private ArtifactPopupService _popupService;
        private UIViewHost _viewHost;
        private ArtifactsConfigComponent _config;
        private ResourcesViewConfigComponent _resourcesViewConfig;
        private Camera _camera;

        [Inject]
        private void Construct(ArtifactsModel model,
                               ResourcesModel resourcesModel,
                               ArtifactPopupService popupService,
                               UIViewHost viewHost)
        {
            _model = model;
            _resourcesModel = resourcesModel;
            _popupService = popupService;
            _viewHost = viewHost;
        }

        public void Setup()
        {
            _config = CMSContainer
                .Get(CMSPath.Configs.ArtifactsConfig)
                .GetComponent<ArtifactsConfigComponent>();

            _resourcesViewConfig = CMSContainer
                .Get(CMSPath.Configs.ResourcesConfig)
                .GetComponent<ResourcesViewConfigComponent>();

            _camera = _viewHost.Canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : _viewHost.Canvas.worldCamera;

            _descriptionPanel.gameObject.SetActive(false);

            foreach (var artifact in _model.Artifacts)
            {
                AddArtifact(artifact);
            }

            foreach (var resourceCost in _model.PurchaseCost)
            {
                AddCost(resourceCost.Key, resourceCost.Value);
            }

            _model.Artifacts
                .ObserveAdd()
                .Subscribe(eventData => AddArtifact(eventData.Value))
                .AddTo(_disposable);

            _model.PurchaseCost
                .ObserveAdd()
                .Subscribe(eventData => AddCost(eventData.Value.Key, eventData.Value.Value))
                .AddTo(_disposable);

            _model.PurchaseCost
                .ObserveDictionaryReplace()
                .Subscribe(eventData => ChangeCost(eventData.Key, eventData.NewValue))
                .AddTo(_disposable);

            _resourcesModel.Resources
                .ObserveAdd()
                .Subscribe(_ => UpdatePurchaseButton())
                .AddTo(_disposable);

            _resourcesModel.Resources
                .ObserveRemove()
                .Subscribe(_ => UpdatePurchaseButton())
                .AddTo(_disposable);

            _resourcesModel.Resources
                .ObserveDictionaryReplace()
                .Subscribe(_ => UpdatePurchaseButton())
                .AddTo(_disposable);

            _purchaseButton.OnClickAsObservable()
                .Subscribe(_ => Purchase())
                .AddTo(_disposable);

            UpdatePurchaseButton();
        }

        private void AddArtifact(CMSEntity artifact)
        {
            var image = Object.Instantiate(_config.ArtifactViewPrefab, _artifactsGroup);
            var component = artifact.GetComponent<ArtifactComponent>();

            image.transform.localScale = Vector3.one * _config.ArtifactScale;
            image.sprite = component.Sprite;
            _popupService.Register(artifact, image.rectTransform);

            image.OnPointerEnterAsObservable()
                .Subscribe(_ => ShowDescription(image.rectTransform, component))
                .AddTo(_disposable);

            image.OnPointerExitAsObservable()
                .Subscribe(_ => _descriptionPanel.gameObject.SetActive(false))
                .AddTo(_disposable);
        }

        private void AddCost(CMSEntity resource, int count)
        {
            var resourceView = Object.Instantiate(_resourcesViewConfig.ResourceViewPrefab, _costGroup);
            resourceView.transform.localScale = Vector3.one * _config.ResourceScale;
            resourceView.Setup(resource.GetComponent<ResourceComponent>().Sprite, count);
            _costViews.Add(resource, resourceView);
        }

        private void ChangeCost(CMSEntity resource, int count)
        {
            if (_costViews.TryGetValue(resource, out var costView))
                costView.SetCount(count);

            UpdatePurchaseButton();
        }

        private void Purchase()
        {
            _model.TryPurchase();
            UpdatePurchaseButton();
        }

        private void ShowDescription(RectTransform artifact, ArtifactComponent component)
        {
            _artifactName.text = component.Name;
            _artifactDescription.text = component.Description;
            _descriptionPanel.gameObject.SetActive(true);

            var screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, artifact.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)_descriptionPanel.parent,
                screenPoint,
                _camera,
                out var localPoint);

            _descriptionPanel.anchoredPosition = localPoint + Vector2.down * artifact.rect.height;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_descriptionPanel);
        }

        private void UpdatePurchaseButton()
        {
            _purchaseButton.interactable = _model.CanPurchase();
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
