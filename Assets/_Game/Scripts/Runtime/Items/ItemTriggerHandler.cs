using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using Game.Runtime.Data.CMSComponents.Item;
using Game.Runtime.Utils;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Runtime.Items
{
    public class ItemTriggerHandler
    {
        private CMSEntity _dataEntity;
        private RectTransform _triggerRoot;
        private Image _parentTrigger;

        public Observable<PointerEventData> OnPointerEnter => _parentTrigger.OnPointerEnterAsObservable();
        public Observable<PointerEventData> OnPointerExit => _parentTrigger.OnPointerExitAsObservable();
        public Observable<PointerEventData> OnBeginDrag => _parentTrigger.OnBeginDragAsObservable();
        public Observable<PointerEventData> OnDrag => _parentTrigger.OnDragAsObservable();
        public Observable<PointerEventData> OnEndDrag => _parentTrigger.OnEndDragAsObservable();
        public RectTransform TriggerRoot => _triggerRoot; 

        public ItemTriggerHandler(CMSEntity dataEntity)
        {
            _dataEntity = dataEntity;
        }

        public void SetupTrigger(RectTransform triggerRoot)
        {
            var grid = _dataEntity.GetComponent<GridCMSComponent>();
            var itemViewConfig = _dataEntity.GetComponent<ItemViewComponent>();

            _triggerRoot = triggerRoot;
            _parentTrigger = _triggerRoot.gameObject.AddComponent<Image>();
            _parentTrigger.raycastTarget = false;
            _parentTrigger.SetAlpha(0);

            foreach (var position in grid.GridPattern)
            {
                AddTrigger(position, itemViewConfig);
            }
        }

        private void AddTrigger(Vector2Int gridPosition, ItemViewComponent itemViewConfig)
        {
            var cellObject = new GameObject($"InventoryCell {gridPosition}", typeof(Image));
            var trigger = cellObject.GetComponent<Image>();
            trigger.SetAlpha(0);
            trigger.rectTransform.SetParent(_triggerRoot);
            trigger.rectTransform.SetAsFirstSibling();
            trigger.rectTransform.localPosition = Vector3.zero;
            trigger.rectTransform.localScale = Vector3.one;
            trigger.rectTransform.sizeDelta = Vector2.one * itemViewConfig.CellSize;
            trigger.rectTransform.anchoredPosition = gridPosition * itemViewConfig.CellSize;
        }
    }
}
