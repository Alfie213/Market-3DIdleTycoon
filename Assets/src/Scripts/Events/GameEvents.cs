using System;

public static class GameEvents
{
    public static event Action<int> OnCurrencyChanged;
    public static event Action<BuildingController> OnBuildingConstructed;
    public static event Action OnShopOpened;
    public static event Action<BuildingController> OnUpgradeWindowRequested;
    
    // --- НОВЫЕ СОБЫТИЯ ДЛЯ ТУТОРИАЛА ---
    public static event Action OnCustomerServed; // Клиент оплатил и ушел
    public static event Action OnUpgradePurchased; // Куплен любой апгрейд

    public static void InvokeCurrencyChanged(int amount) => OnCurrencyChanged?.Invoke(amount);
    public static void InvokeBuildingConstructed(BuildingController building) => OnBuildingConstructed?.Invoke(building);
    public static void InvokeShopOpened() => OnShopOpened?.Invoke();
    public static void InvokeUpgradeWindowRequested(BuildingController building) => OnUpgradeWindowRequested?.Invoke(building);
    
    public static void InvokeCustomerServed() => OnCustomerServed?.Invoke();
    public static void InvokeUpgradePurchased() => OnUpgradePurchased?.Invoke();
}