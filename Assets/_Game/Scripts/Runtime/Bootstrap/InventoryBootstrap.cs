using Game.CMS.Runtime;
using Game.Runtime.Input;
using Game.Runtime.UI.DropPanels;
using Game.Runtime.UI.Inventory;
using Zenject;

namespace Game.Runtime.Bootstrap
{
    public class InventoryBootstrap : IInitializable
    {
        private readonly InventoryModel _inventoryModel;
        private readonly InputService _inputService;
        private readonly InventoryView _inventoryView;
        private readonly SpawnPanel _spawnPanel;

        public InventoryBootstrap(InventoryModel inventoryModel, InventoryView inventoryView, InputService inputService, SpawnPanel spawnPanel)
        {
            _inventoryModel = inventoryModel;
            _inputService = inputService;
            _inventoryView = inventoryView;
            _spawnPanel = spawnPanel;
        }

        public void Initialize()
        {
            CMSContainer.Reload();
            _inputService.Enable();
            _inventoryView.Setup();
            _inventoryModel.GenerateGrid();
            _spawnPanel.SpawnItems();
        }

    }
}
