using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns NPCs based on store progress (dynamic interval).
/// </summary>
public class CustomerSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Customer[] customerPrefabs;
    [SerializeField] private BuildingController[] storeRoute;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform exitPoint;
    
    [Header("Balance")]
    [SerializeField] private float baseSpawnInterval = 6f;
    [SerializeField] private float minSpawnInterval = 1.5f;
    [SerializeField] private float randomness = 1.0f;
    [SerializeField] private float improvementFactor = 0.15f;

    private bool _spawningActive = false;
    private Transform _customersContainer;

    private void Awake()
    {
        _customersContainer = new GameObject("--- CUSTOMERS CONTAINER ---").transform;
    }

    private void OnEnable() => GameEvents.OnShopOpened += StartSpawning;
    private void OnDisable() => GameEvents.OnShopOpened -= StartSpawning;

    private void StartSpawning()
    {
        _spawningActive = true;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(1f);
        while (_spawningActive)
        {
            SpawnCustomer();
            float delay = CalculateDynamicDelay();
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnCustomer()
    {
        if (customerPrefabs.Length == 0) return;
        Customer randomPrefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];
        Customer newCustomer = Instantiate(randomPrefab, spawnPoint.position, Quaternion.identity, _customersContainer);
        newCustomer.Initialize(storeRoute, exitPoint.position);
    }

    private float CalculateDynamicDelay()
    {
        int totalUpgradePoints = 0;
        foreach (var building in storeRoute)
        {
            if (building.IsBuilt)
            {
                totalUpgradePoints += 1;
                if (building.CurrentUnlockedWorkers > 1) totalUpgradePoints += (building.CurrentUnlockedWorkers - 1);
                totalUpgradePoints += building.CurrentSpeedLevel;
            }
        }

        float reductionMultiplier = 1 + (totalUpgradePoints * improvementFactor);
        float calculatedDelay = baseSpawnInterval / reductionMultiplier;
        float randomOffset = Random.Range(-randomness, randomness);
        
        return Mathf.Max(minSpawnInterval, calculatedDelay + randomOffset);
    }
}