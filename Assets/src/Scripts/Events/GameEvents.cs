using System;

public static class GameEvents
{
    // ... старые события ...
    public static event Action<int> OnCurrencyChanged;
    public static event Action<BuildingController> OnBuildingConstructed;
    public static event Action OnShopOpened;
    public static event Action<BuildingController> OnUpgradeWindowRequested;
    public static event Action OnSaleCompleted;
    public static event Action OnUpgradePurchased;
    
    // --- НОВОЕ ---
    public static event Action<int> OnTicketsChanged;
    // -------------

    // ... старые методы ...
    public static void InvokeCurrencyChanged(int amount) => OnCurrencyChanged?.Invoke(amount);
    public static void InvokeBuildingConstructed(BuildingController building) => OnBuildingConstructed?.Invoke(building);
    public static void InvokeShopOpened() => OnShopOpened?.Invoke();
    public static void InvokeUpgradeWindowRequested(BuildingController building) => OnUpgradeWindowRequested?.Invoke(building);
    public static void InvokeSaleCompleted() => OnSaleCompleted?.Invoke();
    public static void InvokeUpgradePurchased() => OnUpgradePurchased?.Invoke();
    
    // --- НОВОЕ ---
    public static void InvokeTicketsChanged(int count) => OnTicketsChanged?.Invoke(count);
    // -------------
}