using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private LayerMask buildingLayer;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, buildingLayer))
        {
            if (hit.collider.TryGetComponent(out BuildingObjectBase building))
            {
                TryBuild(building);
            }
        }
    }

    private void TryBuild(BuildingObjectBase building)
    {
        if (building.IsBuilt) return;

        int cost = building.Data.Cost;
        if (CurrencyController.Instance.TrySpendCurrency(cost))
        {
            building.Construct();
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
}