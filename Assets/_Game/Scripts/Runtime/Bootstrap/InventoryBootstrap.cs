using Game.CMS.Runtime;
using Game.Runtime.Artifacts;
using Game.Runtime.Input;
using Game.Runtime.Resources;
using Game.Runtime.UI;
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
        private readonly SpeedButtonsView _speedButtonsView;
        private readonly ResourceProductionHandler _resourceProductionHandler;
        private readonly ResourcePopupService _resourcePopupService;
        private readonly ArtifactsModel _artifactsModel;
        private readonly ArtifactsView _artifactsView;

        public InventoryBootstrap(InventoryModel inventoryModel,
                                  InventoryView inventoryView,
                                  InputService inputService,
                                  SpawnPanel spawnPanel,
                                  DestructionPanel destructionPanel,
                                  ResourcesView resourcesView,
                                  SpeedButtonsView speedButtonsView,
                                  ResourceProductionHandler resourceProductionHandler,
                                  ResourcePopupService resourcePopupService,
                                  ArtifactsModel artifactsModel,
                                  ArtifactsView artifactsView)
        {
            _inventoryModel = inventoryModel;
            _inputService = inputService;
            _inventoryView = inventoryView;
            _spawnPanel = spawnPanel;
            _destructionPanel = destructionPanel;
            _resourcesView = resourcesView;
            _speedButtonsView = speedButtonsView;
            _resourceProductionHandler = resourceProductionHandler;
            _resourcePopupService = resourcePopupService;
            _artifactsModel = artifactsModel;
            _artifactsView = artifactsView;
        }

        public void Initialize()
        {
            CMSContainer.Reload();
            _inputService.Enable();
            _inventoryView.Setup();
            _inventoryModel.GenerateGrid();
            _resourcesView.Setup();
            _resourcePopupService.Setup();
            _artifactsModel.Setup();
            _artifactsView.Setup();
            _resourceProductionHandler.Setup();
            _speedButtonsView.Setup();
            _destructionPanel.Setup();
            _spawnPanel.SpawnItems();
        }
    }
}
