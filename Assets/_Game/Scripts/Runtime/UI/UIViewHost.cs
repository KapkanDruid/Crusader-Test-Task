using Game.Runtime.UI.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game.Runtime.UI
{
    public class UIViewHost : MonoBehaviour, IDisposable
    {
        [SerializeField] private InventoryView _inventoryView;

        private List<object> _views = new();

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _views.Add(_inventoryView);

            foreach (var view in _views)
            {
                diContainer.Inject(view);
            }
        }

        public void InitializeViews()
        {
            foreach (var view in _views)
            {
                if (view is IInitializable initializable)
                {
                    initializable.Initialize();
                }
            }
        }

        public void Dispose()
        {
            foreach (var view in _views)
            {
                if (view is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}