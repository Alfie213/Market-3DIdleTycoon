using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private List<BuildingObjectBase> requiredBuildings;
    
    private HashSet<BuildingObjectBase> _builtBuildings = new HashSet<BuildingObjectBase>();
    private bool _isOpen = false;

    private void OnEnable()
    {
        GameEvents.OnBuildingConstructed += HandleBuildingConstructed;
    }

    private void OnDisable()
    {
        GameEvents.OnBuildingConstructed -= HandleBuildingConstructed;
    }

    private void HandleBuildingConstructed(BuildingObjectBase building)
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
    }
}