using UnityEngine;

public class UpgradeWindowController : MonoBehaviour
{
    [SerializeField] private UpgradeWindowView view;
    private BuildingController _currentBuilding;

    private void OnEnable()
    {
        GameEvents.OnUpgradeWindowRequested += OpenWindow;
        GameEvents.OnCurrencyChanged += HandleCurrencyChanged;
        
        view.OnCloseClicked += CloseWindow;
        view.OnSpeedUpgradeClicked += TryUpgradeSpeed;
        view.OnWorkersUpgradeClicked += TryUpgradeWorkers;
    }

    private void OnDisable()
    {
        GameEvents.OnUpgradeWindowRequested -= OpenWindow;
        GameEvents.OnCurrencyChanged -= HandleCurrencyChanged;
        
        view.OnCloseClicked -= CloseWindow;
        view.OnSpeedUpgradeClicked -= TryUpgradeSpeed;
        view.OnWorkersUpgradeClicked -= TryUpgradeWorkers;
        
        if (_currentBuilding != null)
        {
            _currentBuilding.OnStatsChanged -= RefreshUI;
        }
    }

    private void HandleCurrencyChanged(int newAmount)
    {
        if (_currentBuilding != null)
        {
            RefreshUI();
        }
    }

    private void OpenWindow(BuildingController building)
    {
        if (_currentBuilding != null) _currentBuilding.OnStatsChanged -= RefreshUI;
        _currentBuilding = building;
        _currentBuilding.OnStatsChanged += RefreshUI;
        
        // --- БЫЛО: view.SetWindowActive(true); ---
        // --- СТАЛО: ---
        view.Show(); 
        // --------------
        
        RefreshUI();
    }

    private void CloseWindow()
    {
        if (_currentBuilding != null)
        {
            _currentBuilding.OnStatsChanged -= RefreshUI;
            _currentBuilding = null;
        }
        
        // --- БЫЛО: view.SetWindowActive(false); ---
        // --- СТАЛО: ---
        view.Hide();
        // --------------
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

        int currentMoney = CurrencyController.Instance.CurrentCurrency;
        int speedCost = _currentBuilding.Data.SpeedUpgradeCost;
        int workerCost = _currentBuilding.Data.WorkerUpgradeCost;

        bool isMaxWorkers = _currentBuilding.CurrentUnlockedWorkers >= _currentBuilding.MaxPossibleWorkers;
        bool isMaxSpeed = _currentBuilding.CurrentSpeedLevel >= _currentBuilding.Data.MaxSpeedUpgrades;
        
        bool canAffordSpeed = currentMoney >= speedCost;
        bool canAffordWorkers = currentMoney >= workerCost;
        
        view.UpdateCosts(
            speedCost,
            workerCost,
            isMaxSpeed,
            isMaxWorkers,
            canAffordSpeed,
            canAffordWorkers
        );
    }

    private void TryUpgradeSpeed()
    {
        if (_currentBuilding == null) return;
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