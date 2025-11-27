using UnityEngine;

public class UpgradeWindowController : MonoBehaviour
{
    [SerializeField] private UpgradeWindowView view;
    private BuildingController _currentBuilding;

    // ... (OnEnable/Disable/OpenWindow/CloseWindow такие же как раньше) ...
    private void OnEnable()
    {
        GameEvents.OnUpgradeWindowRequested += OpenWindow;
        view.OnCloseClicked += CloseWindow;
        view.OnSpeedUpgradeClicked += TryUpgradeSpeed;
        view.OnWorkersUpgradeClicked += TryUpgradeWorkers;
    }

    private void OnDisable()
    {
        GameEvents.OnUpgradeWindowRequested -= OpenWindow;
        view.OnCloseClicked -= CloseWindow;
        view.OnSpeedUpgradeClicked -= TryUpgradeSpeed;
        view.OnWorkersUpgradeClicked -= TryUpgradeWorkers;
        if (_currentBuilding != null) _currentBuilding.OnStatsChanged -= RefreshUI;
    }

    private void OpenWindow(BuildingController building)
    {
        if (_currentBuilding != null) _currentBuilding.OnStatsChanged -= RefreshUI;
        _currentBuilding = building;
        _currentBuilding.OnStatsChanged += RefreshUI;
        view.SetWindowActive(true);
        RefreshUI();
    }

    private void CloseWindow()
    {
        if (_currentBuilding != null)
        {
            _currentBuilding.OnStatsChanged -= RefreshUI;
            _currentBuilding = null;
        }
        view.SetWindowActive(false);
    }

    private void RefreshUI()
    {
        if (_currentBuilding == null) return;

        view.UpdateInfo(_currentBuilding.Data.BuildingName, _currentBuilding.Data.ProfitPerCustomer);
        
        view.UpdateStats(
            _currentBuilding.CurrentProcessingTime,
            _currentBuilding.CurrentUnlockedWorkers,
            _currentBuilding.MaxPossibleWorkers
        );

        // Проверяем оба лимита
        bool isMaxWorkers = _currentBuilding.CurrentUnlockedWorkers >= _currentBuilding.MaxPossibleWorkers;
        
        // Новая проверка для скорости
        bool isMaxSpeed = _currentBuilding.CurrentSpeedLevel >= _currentBuilding.Data.MaxSpeedUpgrades;
        
        // Передаем оба флага во View
        view.UpdateCosts(
            _currentBuilding.Data.SpeedUpgradeCost,
            _currentBuilding.Data.WorkerUpgradeCost,
            isMaxSpeed, // <-- Новое
            isMaxWorkers
        );
    }
    
    // ... (TryUpgradeSpeed/Workers такие же как раньше) ...
    private void TryUpgradeSpeed()
    {
        if (_currentBuilding == null) return;
        
        // Доп. проверка (на всякий случай, хотя кнопка будет выключена)
        if (_currentBuilding.CurrentSpeedLevel >= _currentBuilding.Data.MaxSpeedUpgrades) return;

        if (CurrencyController.Instance.TrySpendCurrency(_currentBuilding.Data.SpeedUpgradeCost))
        {
            _currentBuilding.UpgradeSpeed();
        }
    }

    private void TryUpgradeWorkers()
    {
        if (_currentBuilding == null) return;
        if (CurrencyController.Instance.TrySpendCurrency(_currentBuilding.Data.WorkerUpgradeCost))
        {
            _currentBuilding.UpgradeWorkers();
        }
    }
}