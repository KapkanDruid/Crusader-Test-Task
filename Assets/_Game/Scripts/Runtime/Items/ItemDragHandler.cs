using Game.Runtime.DragAndDrop;
using Game.Runtime.Input;
using Game.Runtime.Utils;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Runtime.Items
{
    public class ItemDragHandler : IDisposable
    {
        private readonly ItemTriggerHandler _triggerHandler;
        private readonly InputService _inputService;
        private readonly DragService _dragService;
        private readonly ItemBehavior _behavior;

        private RectTransform _dragArea;
        private RectTransform _itemRoot;
        private Camera _camera;

        private ItemRotation _currentRotation;
        private Vector2 _lastScreenPosition;
        private Vector2 _dragStartPosition;
        private bool _isDragging;

        private readonly Dictionary<ItemRotation, float> _presetAngles = new()
        {
            [ItemRotation.Up] = 0f,
            [ItemRotation.Right] = -90f,
            [ItemRotation.Down] = 180f,
            [ItemRotation.Left] = 90f
        };

        private readonly CompositeDisposable _disposable = new();
        private readonly Subject<Unit> _onDragStarted = new();
        public bool IsDraggable { get; set; }
        public Observable<Unit> OnDragStarted => _onDragStarted;
        public ItemRotation CurrentRotation => _currentRotation; 

        public ItemDragHandler(ItemTriggerHandler triggerHandler,
                               InputService inputService,
                               ItemBehavior behavior,
                               DragService dragService)
        {
            _triggerHandler = triggerHandler;
            _inputService = inputService;
            _dragService = dragService;
            _behavior = behavior;
        }

        public void Setup(RectTransform dragArea, RectTransform itemRoot)
        {
            _dragArea = dragArea;
            _itemRoot = itemRoot;

            var canvas = _triggerHandler.TriggerRoot.GetComponentInParent<Canvas>(true);
            _camera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

            _triggerHandler.OnBeginDrag
                .Subscribe(OnBeginDrag)
                .AddTo(_disposable);

            _triggerHandler.OnDrag
                .Subscribe(OnDrag)
                .AddTo(_disposable);

            _triggerHandler.OnEndDrag
                .Subscribe(OnEndDrag)
                .AddTo(_disposable);

            _inputService.OnRotateItem
                .Subscribe(_ => OnRotate())
                .AddTo(_disposable);
        }

        private void OnBeginDrag(PointerEventData eventData)
        {
            if (_isDragging || !IsDraggable)
                return;

            if (eventData.button != PointerEventData.InputButton.Left) 
                return;

            _isDragging = true;
            _lastScreenPosition = eventData.position;
            _dragStartPosition = _itemRoot.position;
            _onDragStarted.OnNext(Unit.Default);
            _itemRoot.SetParent(_dragArea);
            _itemRoot.SetAsLastSibling();

            _dragService.StartDrag(_behavior);
            _dragService.Drag(_behavior);
        }

        private void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging)
                return;

            if (!IsInsideDragArea())
            {
                OnEndDrag(eventData);
                return;
            }

            _lastScreenPosition = eventData.position;
            HandlePosition();
            _dragService.Drag(_behavior);
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!_isDragging)
                return;

            _isDragging = false;
            if (!_dragService.TryPlace(_behavior))
            {
                _itemRoot.position = _dragStartPosition;
                _dragService.Return(_behavior);
            }
        }

        private void OnRotate()
        {
            if (!_isDragging)
                return;

            _currentRotation = _currentRotation switch
            {
                ItemRotation.Up => ItemRotation.Right,
                ItemRotation.Right => ItemRotation.Down,
                ItemRotation.Down => ItemRotation.Left,
                ItemRotation.Left => ItemRotation.Up,
                _ => ItemRotation.Up
            };

            _itemRoot.localRotation = Quaternion.Euler(0, 0, _presetAngles[_currentRotation]);

            HandlePosition();
            _dragService.Drag(_behavior);
        }

        private void HandlePosition()
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_dragArea, _lastScreenPosition,
                _camera, out var localPoint))
            {
                _itemRoot.anchoredPosition = localPoint;
            }
        }

        private bool IsInsideDragArea()
        {
            return _dragArea.IsTargetInsideRectTransform(_itemRoot);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
