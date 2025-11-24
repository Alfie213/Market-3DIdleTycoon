using System;

public static class GameEvents
{
    public static event Action<int> OnCurrencyChanged;
    public static event Action<BuildingObjectBase> OnBuildingConstructed;
    public static event Action OnShopOpened;
    
    public static void InvokeCurrencyChanged(int currentAmount) => OnCurrencyChanged?.Invoke(currentAmount);
    public static void InvokeBuildingConstructed(BuildingObjectBase building) => OnBuildingConstructed?.Invoke(building);
    public static void InvokeShopOpened() => OnShopOpened?.Invoke();
}