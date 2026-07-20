using DG.Tweening;
using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Data.CMSComponents.Resources;
using Game.Runtime.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Runtime.Resources
{
    public class ResourcePopupService : IDisposable
    {
        private readonly UIViewHost _viewHost;

        private ResourcesViewConfigComponent _viewConfig;
        private ObjectPool<GameObject> _popupPool;
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
            _popupPool = new ObjectPool<GameObject>(CreatePopup, ShowPopup, HidePopup, DestroyPopup);
        }

        public void Show(RectTransform point, CMSEntity resource, int count)
        {
            var popup = _popupPool.Get();
            var popupTransform = popup.GetComponent<RectTransform>();
            var canvasGroup = popup.GetComponent<CanvasGroup>();

            var screenPoint = RectTransformUtility.WorldToScreenPoint(_camera, point.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _viewHost.ResourcePopupRoot,
                screenPoint,
                _camera,
                out var localPoint);

            popupTransform.anchoredPosition = localPoint;
            popup.GetComponent<Image>().sprite = resource.GetComponent<ResourceComponent>().Sprite;
            popup.GetComponentInChildren<TMP_Text>().text = count.ToString();

            DOTween.Sequence(popup)
                .Append(popupTransform.DOAnchorPos(localPoint + _viewConfig.ResourcePopupOffset,
                    _viewConfig.ResourcePopupDuration))
                .Join(canvasGroup.DOFade(0f, _viewConfig.ResourcePopupDuration))
                .OnComplete(() => _popupPool.Release(popup));
        }

        private GameObject CreatePopup()
        {
            var popup = Object.Instantiate(_viewConfig.ResourceViewPrefab, _viewHost.ResourcePopupRoot);
            var popupTransform = popup.GetComponent<RectTransform>();

            popup.name = "ResourcePopup";
            popupTransform.anchorMin = _viewHost.ResourcePopupRoot.pivot;
            popupTransform.anchorMax = _viewHost.ResourcePopupRoot.pivot;
            popupTransform.localScale = Vector3.one * _viewConfig.ResourceScale;
            popup.AddComponent<CanvasGroup>();

            foreach (var graphic in popup.GetComponentsInChildren<Graphic>(true))
            {
                graphic.raycastTarget = false;
            }

            return popup;
        }

        private void ShowPopup(GameObject popup)
        {
            DOTween.Kill(popup);
            popup.GetComponent<CanvasGroup>().alpha = 1f;
            popup.SetActive(true);
        }

        private void HidePopup(GameObject popup)
        {
            popup.SetActive(false);
        }

        private void DestroyPopup(GameObject popup)
        {
            DOTween.Kill(popup);
            Object.Destroy(popup);
        }

        public void Dispose()
        {
            _popupPool.Clear();
        }
    }
}
