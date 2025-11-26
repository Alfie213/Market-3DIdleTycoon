using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance { get; private set; }

    [SerializeField] private List<BuildingController> requiredBuildings;
    
    private HashSet<BuildingController> _builtBuildings = new HashSet<BuildingController>();
    private bool _isOpen = false;

    // Публичное свойство для проверки статуса
    public bool IsShopOpen => _isOpen;

    private void Awake()
    {
        // Базовая реализация Singleton для сцены
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameEvents.OnBuildingConstructed += HandleBuildingConstructed;
    }

    private void OnDisable()
    {
        GameEvents.OnBuildingConstructed -= HandleBuildingConstructed;
    }

    private void HandleBuildingConstructed(BuildingController building)
    {
        if (_isOpen) return;
        if (!requiredBuildings.Contains(building)) return;

        _builtBuildings.Add(building);
        CheckOpenCondition();
    }

    private void CheckOpenCondition()
    {
        // Проверяем, все ли обязательные здания есть в списке построенных
        foreach (var required in requiredBuildings)
        {
            if (!_builtBuildings.Contains(required)) return;
        }

        OpenShop();
    }

    private void OpenShop()
    {
        _isOpen = true;
        GameEvents.InvokeShopOpened();
        Debug.Log("Shop is now OPEN! Customers started coming.");
    }
}