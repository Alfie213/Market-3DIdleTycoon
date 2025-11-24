using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<int> OnCurrencyChanged;
    public static event Action<BuildingController> OnBuildingConstructed;
    public static event Action OnShopOpened;
    
    // Событие: Игрок кликнул на уже построенное здание, нужно открыть UI
    public static event Action<BuildingController> OnUpgradeWindowRequested; 
    
    public static void InvokeCurrencyChanged(int amount) => OnCurrencyChanged?.Invoke(amount);
    public static void InvokeBuildingConstructed(BuildingController building) => OnBuildingConstructed?.Invoke(building);
    public static void InvokeShopOpened() => OnShopOpened?.Invoke();
    public static void InvokeUpgradeWindowRequested(BuildingController building) => OnUpgradeWindowRequested?.Invoke(building);
}