using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Customer[] customerPrefabs;
    [SerializeField] private BuildingController[] storeRoute;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform exitPoint;
    
    [Header("Balance")]
    [Tooltip("Базовое время между покупателями (когда ничего не прокачано)")]
    [SerializeField] private float baseSpawnInterval = 6f;
    
    [Tooltip("Минимально возможное время между спавном (кап скорости)")]
    [SerializeField] private float minSpawnInterval = 1.5f;
    
    [Tooltip("Разброс времени. Если 1, то к времени добавится от -1 до +1 сек")]
    [SerializeField] private float randomness = 1.0f;

    [Tooltip("Насколько сильно каждое улучшение ускоряет спавн (0.1 = 10% от базы за каждый уровень)")]
    [SerializeField] private float improvementFactor = 0.15f;

    private bool _spawningActive = false;
    private Transform _customersContainer;

    private void Awake()
    {
        // Создаем "папку" для NPC, чтобы не мусорить в иерархии
        _customersContainer = new GameObject("--- CUSTOMERS CONTAINER ---").transform;
    }

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
        // Небольшая задержка перед самым первым клиентом
        yield return new WaitForSeconds(1f);

        while (_spawningActive)
        {
            SpawnCustomer();

            // Вычисляем задержку перед СЛЕДУЮЩИМ клиентом
            float delay = CalculateDynamicDelay();
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnCustomer()
    {
        if (customerPrefabs.Length == 0) return;

        Customer randomPrefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];
        
        // Спавним ВНУТРИ контейнера
        Customer newCustomer = Instantiate(randomPrefab, spawnPoint.position, Quaternion.identity, _customersContainer);
        
        newCustomer.Initialize(storeRoute, exitPoint.position);
    }

    // Логика расчета скорости потока людей
    private float CalculateDynamicDelay()
    {
        int totalUpgradePoints = 0;

        foreach (var building in storeRoute)
        {
            if (building.IsBuilt)
            {
                // 1 очко за само здание
                totalUpgradePoints += 1;

                // Очки за дополнительных работников (начинаем с 0, поэтому -1 не нужно, если UnlockedWorkers = 1)
                // Если UnlockedWorkers = 1, то доп. очков 0. Если 2, то +1 очко.
                if (building.CurrentUnlockedWorkers > 1)
                {
                    totalUpgradePoints += (building.CurrentUnlockedWorkers - 1);
                }

                // Очки за уровни скорости
                totalUpgradePoints += building.CurrentSpeedLevel;
            }
        }

        // Формула уменьшения времени: Base / (1 + (Points * Factor))
        // Пример: 5 очков * 0.15 = 0.75. Делитель = 1.75. Время: 6 / 1.75 = 3.4 сек.
        float reductionMultiplier = 1 + (totalUpgradePoints * improvementFactor);
        float calculatedDelay = baseSpawnInterval / reductionMultiplier;

        // Добавляем случайность (Рандом от -1 до 1)
        float randomOffset = Random.Range(-randomness, randomness);
        
        // Итоговое время, но не меньше минимума
        return Mathf.Max(minSpawnInterval, calculatedDelay + randomOffset);
    }
}