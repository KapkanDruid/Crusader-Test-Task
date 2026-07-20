using DG.Tweening;
using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Data.CMSComponents.Resources;
using Game.Runtime.UI;
using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Game.Runtime.Resources
{
    public class ResourcePopupService : IDisposable
    {
        private readonly UIViewHost _viewHost;

        private ResourcesViewConfigComponent _viewConfig;
        private ObjectPool<ResourceGroupElement> _popupPool;
        private Camera _camera;

        public ResourcePopupService(UIViewHost viewHost)
        {
            _viewHost = viewHost;
        }

        public void Setup()
        {
            _viewConfig = CMSContainer
                .Get(CMSPath.Configs.ResourcesConfig)
                .GetComponent<ResourcesViewConfigComponent>();

            _camera = _viewHost.Canvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : _viewHost.Canvas.worldCamera;

            _viewHost.ResourcePopupRoot.SetAsLastSibling();
            _popupPool = new ObjectPool<ResourceGroupElement>(CreatePopup, ShowPopup, HidePopup, DestroyPopup);
        }

        public void Show(RectTransform point, CMSEntity resource, int count)
        {
            var popup = _popupPool.Get();
            var popupTransform = popup.RectTransform;
            var canvasGroup = popup.CanvasGroup;

            var screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, point.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _viewHost.ResourcePopupRoot,
                screenPoint,
                _camera,
                out var localPoint);

            popupTransform.anchoredPosition = localPoint;
            popup.Setup(resource.GetComponent<ResourceComponent>().Sprite, count);

            DOTween.Sequence(popup)
                .Append(popupTransform.DOAnchorPos(localPoint + _viewConfig.ResourcePopupOffset,
                    _viewConfig.ResourcePopupDuration))
                .Join(canvasGroup.DOFade(0f, _viewConfig.ResourcePopupDuration))
                .OnComplete(() => _popupPool.Release(popup));
        }

        private ResourceGroupElement CreatePopup()
        {
            var popup = Object.Instantiate(_viewConfig.ResourceViewPrefab, _viewHost.ResourcePopupRoot);
            var popupTransform = popup.RectTransform;

            popup.name = "ResourcePopup";
            popupTransform.anchorMin = _viewHost.ResourcePopupRoot.pivot;
            popupTransform.anchorMax = _viewHost.ResourcePopupRoot.pivot;
            popupTransform.localScale = Vector3.one * _viewConfig.ResourceScale;

            return popup;
        }

        private void ShowPopup(ResourceGroupElement popup)
        {
            DOTween.Kill(popup);
            popup.CanvasGroup.alpha = 1f;
            popup.gameObject.SetActive(true);
        }

        private void HidePopup(ResourceGroupElement popup)
        {
            popup.gameObject.SetActive(false);
        }

        private void DestroyPopup(ResourceGroupElement popup)
        {
            if (popup == null)
                return;

            DOTween.Kill(popup);
            Object.Destroy(popup.gameObject);
        }

        public void Dispose()
        {
            _popupPool.Clear();
        }
    }
}
