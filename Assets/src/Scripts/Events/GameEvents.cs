using System;

/// <summary>
/// Static Event Bus for global game communication.
/// Decouples systems by allowing them to broadcast notifications without knowing the listeners.
/// </summary>
public static class GameEvents
{
    // Economy events
    public static event Action<int> OnCurrencyChanged;
    public static event Action<int> OnTicketsChanged;

    // Building & Gameplay events
    public static event Action<BuildingController> OnBuildingConstructed;
    public static event Action<BuildingController> OnUpgradeWindowRequested;
    public static event Action OnUpgradePurchased;
    
    /// <summary>
    /// Triggered when the shop is fully open (all required buildings are built).
    /// </summary>
    public static event Action OnShopOpened;

    /// <summary>
    /// Triggered when a customer successfully pays for a product.
    /// Used for audio, inventory drops, and tutorial steps.
    /// </summary>
    public static event Action OnSaleCompleted;

    #region Invokers
    public static void InvokeCurrencyChanged(int amount) => OnCurrencyChanged?.Invoke(amount);
    public static void InvokeTicketsChanged(int count) => OnTicketsChanged?.Invoke(count);
    public static void InvokeBuildingConstructed(BuildingController building) => OnBuildingConstructed?.Invoke(building);
    public static void InvokeUpgradeWindowRequested(BuildingController building) => OnUpgradeWindowRequested?.Invoke(building);
    public static void InvokeUpgradePurchased() => OnUpgradePurchased?.Invoke();
    public static void InvokeShopOpened() => OnShopOpened?.Invoke();
    public static void InvokeSaleCompleted() => OnSaleCompleted?.Invoke();
    #endregion
}