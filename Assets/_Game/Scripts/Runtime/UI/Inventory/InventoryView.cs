using Game.CMS.Runtime;
using Game.Runtime.Data.CMSComponents.Config;
using ModestTree.Util;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = UnityEngine.Object;

namespace Game.Runtime.UI.Inventory
{
    [Serializable]
    public class InventoryView : IInitializable, IDisposable
    {
        [SerializeField] private RectTransform _inventoryRoot;

        private InventoryModel _model;
        private IInventoryViewCommands _commands;

        private InventoryViewConfigComponent _config;
        private readonly CompositeDisposable _disposable = new();
        private readonly Dictionary<Vector2Int, RectTransform> _cells = new();

        [Inject]
        private void Construct(InventoryModel model, IInventoryViewCommands commands)
        {
            _model = model;
            _commands = commands;
        }

        public void Initialize()
        {
            _config = CMSContainer.Get(CMSPath.Configs.InventoryConfig).GetComponent<InventoryViewConfigComponent>();

            _model.GridPositions
                .ObserveAdd()
                .Select(eventData => eventData.Value)
                .Subscribe(AddCell)
                .AddTo(_disposable);

            _model.GridPositions
                .ObserveRemove()
                .Select(eventData => eventData.Value)
                .Subscribe(RemoveCel)
                .AddTo(_disposable);
        }

        private void AddCell(Vector2Int gridPosition)
        {
            var cellObject = new GameObject($"InventoryCell {gridPosition}", typeof(RectTransform));
            var cell = cellObject.GetComponent<RectTransform>();

            cell.SetParent(_inventoryRoot);
            cell.SetAsFirstSibling();
            cell.localPosition = Vector3.zero;
            cell.localScale = Vector3.one;
            cell.sizeDelta = Vector2.one * _config.CellSize;
            cell.anchoredPosition = gridPosition * _config.CellSize;

            var imageObject = new GameObject($"InventoryCellImage {gridPosition}", typeof(Image));
            var image = imageObject.GetComponent<Image>();

            image.rectTransform.SetParent(cell);
            image.rectTransform.localPosition = Vector3.zero;
            image.rectTransform.localScale = Vector3.one;
            image.rectTransform.anchoredPosition = Vector3.zero;

            image.sprite = _config.CellSprite;
            image.rectTransform.sizeDelta = Vector2.one * _config.CellImageSize;

            _commands.SetCell(gridPosition, cell);
            _cells[gridPosition] = cell;
        }

        private void RemoveCel(Vector2Int gridPosition)
        {
            _commands.RemoveCell(gridPosition);
            Object.Destroy(_cells[gridPosition].gameObject);
            _cells.Remove(gridPosition);
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}