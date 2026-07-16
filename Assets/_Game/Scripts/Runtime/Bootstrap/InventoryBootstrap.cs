using Game.CMS.Runtime;
using Game.Runtime.UI;
using Game.Runtime.UI.Inventory;
using System;
using Zenject;

namespace Game.Runtime.Bootstrap
{
    public class InventoryBootstrap : IInitializable
    {
        private readonly InventoryModel _inventoryModel;
        private readonly UIViewHost _uIViewHost;

        public InventoryBootstrap(InventoryModel inventoryModel, UIViewHost uIViewHost)
        {
            _inventoryModel = inventoryModel;
            _uIViewHost = uIViewHost;
        }

        public void Initialize()
        {
            CMSContainer.Reload();
            _uIViewHost.InitializeViews();
            _inventoryModel.GenerateGrid();
        }
    }
}
