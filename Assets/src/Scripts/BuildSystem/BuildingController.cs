using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;
    
    [Header("Visuals")]
    [SerializeField] private GameObject visualModel;
    [SerializeField] private GameObject constructionSiteVisuals;
    
    [Header("Interaction")]
    [SerializeField] private Transform interactionPoint; // Точка входа в здание
    // QueueStartPoint больше не нужен здесь, он теперь внутри WorkerPoint
    
    [Header("Workers")]
    [SerializeField] private List<WorkerPoint> workerPoints; 

    public event Action OnStatsChanged;

    private bool _isBuilt = false;
    
    // Текущие статы (храним тут, чтобы раздавать работникам)
    private int _currentUnlockedWorkers;
    private float _currentProcessingTime;

    public BuildingData Data => buildingData;
    public bool IsBuilt => _isBuilt;
    public Transform InteractionPoint => interactionPoint;
    
    public int CurrentUnlockedWorkers => _currentUnlockedWorkers;
    public int MaxPossibleWorkers => buildingData.MaxPossibleWorkers;
    public float CurrentProcessingTime => _currentProcessingTime;
    
    public int ActiveWorkersCount 
    {
        get 
        {
            int count = 0;
            foreach (var wp in workerPoints) if (wp.IsBusy) count++;
            return count;
        }
    }

    private void Awake()
    {
        _currentUnlockedWorkers = Mathf.Min(buildingData.BaseWorkers, buildingData.MaxPossibleWorkers);
        _currentProcessingTime = buildingData.BaseProcessingTime;
        
        // Подписываемся на изменения каждого работника, чтобы обновлять UI
        foreach (var wp in workerPoints)
        {
            wp.OnStateChanged += () => OnStatsChanged?.Invoke();
        }

        UpdateVisuals();
    }

    private void Start()
    {
        RefreshWorkerPoints();
    }

    public void Interact()
    {
        if (!_isBuilt) TryConstruct();
        else GameEvents.InvokeUpgradeWindowRequested(this);
    }

    private void TryConstruct()
    {
        if (CurrencyController.Instance.TrySpendCurrency(buildingData.BuildCost))
        {
            _isBuilt = true;
            UpdateVisuals();
            RefreshWorkerPoints(); 
            GameEvents.InvokeBuildingConstructed(this);
        }
    }

    public void UpgradeSpeed()
    {
        _currentProcessingTime = Mathf.Max(0.1f, _currentProcessingTime * 0.9f);
        // Обновляем данные всем работникам
        RefreshWorkerPoints();
        OnStatsChanged?.Invoke();
    }

    public void UpgradeWorkers()
    {
        if (_currentUnlockedWorkers >= buildingData.MaxPossibleWorkers) return;

        _currentUnlockedWorkers++;
        RefreshWorkerPoints();
        OnStatsChanged?.Invoke();
    }

    // Раздаем актуальные статы и включаем/выключаем точки
    private void RefreshWorkerPoints()
    {
        if (!_isBuilt)
        {
            foreach (var wp in workerPoints) wp.SetUnlocked(false);
            return;
        }

        for (int i = 0; i < workerPoints.Count; i++)
        {
            bool shouldBeActive = i < _currentUnlockedWorkers;
            
            // Передаем скорость и профит (на случай если апгрейдили)
            workerPoints[i].Initialize(_currentProcessingTime, buildingData.ProfitPerCustomer);
            workerPoints[i].SetUnlocked(shouldBeActive);
        }
    }

    private void UpdateVisuals()
    {
        if (visualModel) visualModel.SetActive(_isBuilt);
        if (constructionSiteVisuals) constructionSiteVisuals.SetActive(!_isBuilt);
    }

    // --- Логика Распределения Клиентов ---

    public bool CanAcceptCustomer()
    {
        if (!_isBuilt) return false;

        // Если есть хотя бы одна очередь, где место не забито до отказа
        foreach (var wp in workerPoints)
        {
            if (wp.IsUnlocked && wp.QueueCount < buildingData.MaxQueueCapacity)
            {
                return true;
            }
        }
        return false;
    }

    public void EnqueueCustomer(Customer customer)
    {
        WorkerPoint bestPoint = GetBestWorkerPoint();
        
        if (bestPoint != null)
        {
            bestPoint.EnqueueCustomer(customer);
        }
        else
        {
            // Критическая ситуация (не должно случаться при проверке CanAcceptCustomer)
            Debug.LogWarning("No worker points available!");
        }
    }

    // Ищем самую свободную кассу
    private WorkerPoint GetBestWorkerPoint()
    {
        WorkerPoint bestPoint = null;
        int minQueue = int.MaxValue;

        foreach (var wp in workerPoints)
        {
            if (!wp.IsUnlocked) continue;

            // Если нашли пустую кассу - сразу туда
            if (wp.QueueCount == 0) return wp;

            if (wp.QueueCount < minQueue && wp.QueueCount < buildingData.MaxQueueCapacity)
            {
                minQueue = wp.QueueCount;
                bestPoint = wp;
            }
        }

        return bestPoint;
    }
}