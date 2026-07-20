using Game.CMS.Runtime;
using Game.Runtime.Input;
using Game.Runtime.Resources;
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
        private readonly DestructionPanel _destructionPanel;
        private readonly ResourcesView _resourcesView;
        private readonly ResourceProductionHandler _resourceProductionHandler;
        private readonly ResourcePopupService _resourcePopupService;

        public InventoryBootstrap(InventoryModel inventoryModel,
                                  InventoryView inventoryView,
                                  InputService inputService,
                                  SpawnPanel spawnPanel,
                                  DestructionPanel destructionPanel,
                                  ResourcesView resourcesView,
                                  ResourceProductionHandler resourceProductionHandler,
                                  ResourcePopupService resourcePopupService)
        {
            _inventoryModel = inventoryModel;
            _inputService = inputService;
            _inventoryView = inventoryView;
            _spawnPanel = spawnPanel;
            _destructionPanel = destructionPanel;
            _resourcesView = resourcesView;
            _resourceProductionHandler = resourceProductionHandler;
            _resourcePopupService = resourcePopupService;
        }

        public void Initialize()
        {
            CMSContainer.Reload();
            _inputService.Enable();
            _inventoryView.Setup();
            _inventoryModel.GenerateGrid();
            _resourcesView.Setup();
            _resourcePopupService.Setup();
            _resourceProductionHandler.Setup();
            _destructionPanel.Setup();
            _spawnPanel.SpawnItems();
        }
    }
}
