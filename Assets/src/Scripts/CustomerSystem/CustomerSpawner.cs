using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private Customer customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float spawnInterval = 3f;
    
    [Header("Route Configuration")]
    [SerializeField] private VegetableStall vegetableStall;
    [SerializeField] private CashierBuilding cashierBuilding;

    private bool _spawningActive = false;

    private void OnEnable()
    {
        GameEvents.OnShopOpened += StartSpawning;
    }

    private void OnDisable()
    {
        GameEvents.OnShopOpened -= StartSpawning;
    }

    private void StartSpawning()
    {
        _spawningActive = true;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (_spawningActive)
        {
            SpawnCustomer();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnCustomer()
    {
        Customer newCustomer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        
        List<BuildingObjectBase> route = new List<BuildingObjectBase>
        {
            vegetableStall,
            cashierBuilding
        };

        newCustomer.Initialize(route, exitPoint.position);
    }
}