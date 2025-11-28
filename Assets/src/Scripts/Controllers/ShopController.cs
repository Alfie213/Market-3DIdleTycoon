using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks built buildings and determines if the shop is "Open" for customers.
/// </summary>
public class ShopController : MonoBehaviour
{
    public static ShopController Instance { get; private set; }

    [SerializeField] private List<BuildingController> requiredBuildings;
    
    private readonly HashSet<BuildingController> _builtBuildings = new HashSet<BuildingController>();
    private bool _isOpen = false;

    public bool IsShopOpen => _isOpen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable() => GameEvents.OnBuildingConstructed += HandleBuildingConstructed;
    private void OnDisable() => GameEvents.OnBuildingConstructed -= HandleBuildingConstructed;

    private void HandleBuildingConstructed(BuildingController building)
    {
        if (_isOpen) return;
        if (!requiredBuildings.Contains(building)) return;

        _builtBuildings.Add(building);
        CheckOpenCondition();
    }

    private void CheckOpenCondition()
    {
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
        Debug.Log("Shop is OPEN!");
    }
}