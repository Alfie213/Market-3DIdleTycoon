using UnityEngine;

public class UpgradeWindowController : MonoBehaviour
{
    [SerializeField] private UpgradeWindowView view;
    private BuildingController _currentBuilding;

    private void OnEnable()
    {
        GameEvents.OnUpgradeWindowRequested += OpenWindow;
        
        // Подписываемся на изменение валюты, чтобы обновлять кнопки "на лету"
        GameEvents.OnCurrencyChanged += HandleCurrencyChanged;
        
        view.OnCloseClicked += CloseWindow;
        view.OnSpeedUpgradeClicked += TryUpgradeSpeed;
        view.OnWorkersUpgradeClicked += TryUpgradeWorkers;
    }

    private void OnDisable()
    {
        GameEvents.OnUpgradeWindowRequested -= OpenWindow;
        
        // Отписываемся
        GameEvents.OnCurrencyChanged -= HandleCurrencyChanged;
        
        view.OnCloseClicked -= CloseWindow;
        view.OnSpeedUpgradeClicked -= TryUpgradeSpeed;
        view.OnWorkersUpgradeClicked -= TryUpgradeWorkers;
        
        if (_currentBuilding != null)
        {
            _currentBuilding.OnStatsChanged -= RefreshUI;
        }
    }

    // Обработчик изменения денег просто вызывает перерисовку UI
    private void HandleCurrencyChanged(int newAmount)
    {
        // Обновляем UI только если окно открыто
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

        // Получаем текущие данные
        int currentMoney = CurrencyController.Instance.CurrentCurrency;
        int speedCost = _currentBuilding.Data.SpeedUpgradeCost;
        int workerCost = _currentBuilding.Data.WorkerUpgradeCost;

        bool isMaxWorkers = _currentBuilding.CurrentUnlockedWorkers >= _currentBuilding.MaxPossibleWorkers;
        bool isMaxSpeed = _currentBuilding.CurrentSpeedLevel >= _currentBuilding.Data.MaxSpeedUpgrades;
        
        // Проверяем, хватает ли денег
        bool canAffordSpeed = currentMoney >= speedCost;
        bool canAffordWorkers = currentMoney >= workerCost;
        
        // Передаем все данные во View
        view.UpdateCosts(
            speedCost,
            workerCost,
            isMaxSpeed,
            isMaxWorkers,
            canAffordSpeed,    // <-- Новое
            canAffordWorkers   // <-- Новое
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