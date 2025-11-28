using System;

/// <summary>
/// Global Event Bus. Handles communication between disparate systems.
/// </summary>
public static class GameEvents
{
    // Economy
    public static event Action<int> OnCurrencyChanged;
    public static event Action<int> OnTicketsChanged;

    // Gameplay Flow
    public static event Action OnShopOpened;
    public static event Action OnSaleCompleted; // Customer paid
    
    // Construction & Upgrades
    public static event Action<BuildingController> OnBuildingConstructed;
    public static event Action<BuildingController> OnUpgradeWindowRequested;
    public static event Action OnUpgradePurchased;

    #region Invokers
    public static void InvokeCurrencyChanged(int amount) => OnCurrencyChanged?.Invoke(amount);
    public static void InvokeTicketsChanged(int count) => OnTicketsChanged?.Invoke(count);
    public static void InvokeShopOpened() => OnShopOpened?.Invoke();
    public static void InvokeSaleCompleted() => OnSaleCompleted?.Invoke();
    public static void InvokeBuildingConstructed(BuildingController building) => OnBuildingConstructed?.Invoke(building);
    public static void InvokeUpgradeWindowRequested(BuildingController building) => OnUpgradeWindowRequested?.Invoke(building);
    public static void InvokeUpgradePurchased() => OnUpgradePurchased?.Invoke();
    #endregion
}