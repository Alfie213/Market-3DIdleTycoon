using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Customer[] customerPrefabs; // Массив разных скинов NPC
    [SerializeField] private BuildingController[] storeRoute; // Маршрут: Лавка -> Касса
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float spawnInterval = 3f;

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
        if (customerPrefabs.Length == 0) return;

        // 1. Случайный префаб
        Customer randomPrefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];
        Customer newCustomer = Instantiate(randomPrefab, spawnPoint.position, Quaternion.identity);
        
        // 2. Инициализация маршрутом (массивом)
        newCustomer.Initialize(storeRoute, exitPoint.position);
    }
}