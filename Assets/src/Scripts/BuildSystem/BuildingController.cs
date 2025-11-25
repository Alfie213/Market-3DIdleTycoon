using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;
    
    [Header("Visuals")]
    [SerializeField] private GameObject visualModel;
    [SerializeField] private GameObject constructionSiteVisuals;
    
    [Header("Configuration")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private Transform queueStartPoint;
    [Tooltip("Список точек, где стоят работники. Размер списка ограничивает макс. прокачку.")]
    [SerializeField] private List<WorkerPoint> workerPoints; 

    public event Action OnStatsChanged;

    private bool _isBuilt = false;
    private readonly Queue<Customer> _customerQueue = new Queue<Customer>();
    
    private int _currentUnlockedWorkers;
    private float _currentProcessingTime;

    public BuildingData Data => buildingData;
    public bool IsBuilt => _isBuilt;
    public Transform InteractionPoint => interactionPoint;
    
    public int CurrentUnlockedWorkers => _currentUnlockedWorkers;
    public int MaxPossibleWorkers => buildingData.MaxPossibleWorkers;
    public float CurrentProcessingTime => _currentProcessingTime;
    // Считаем занятых, опрашивая точки
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
        // Начинаем с базового кол-ва, но не больше, чем точек на сцене
        _currentUnlockedWorkers = Mathf.Min(buildingData.BaseWorkers, buildingData.MaxPossibleWorkers);
        _currentProcessingTime = buildingData.BaseProcessingTime;
        
        UpdateVisuals();
    }

    private void Start()
    {
        // Инициализация точек при старте
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
            RefreshWorkerPoints(); // Показываем работников после постройки
            GameEvents.InvokeBuildingConstructed(this);
        }
    }

    public void UpgradeSpeed()
    {
        _currentProcessingTime = Mathf.Max(0.1f, _currentProcessingTime * 0.9f);
        OnStatsChanged?.Invoke();
    }

    public void UpgradeWorkers()
    {
        if (_currentUnlockedWorkers >= buildingData.MaxPossibleWorkers) return;

        _currentUnlockedWorkers++;
        RefreshWorkerPoints();
        TryProcessNextCustomer();
        OnStatsChanged?.Invoke();
    }

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
            workerPoints[i].SetUnlocked(shouldBeActive);
        }
    }

    private void UpdateVisuals()
    {
        if (visualModel) visualModel.SetActive(_isBuilt);
        if (constructionSiteVisuals) constructionSiteVisuals.SetActive(!_isBuilt);
    }

    // --- Очередь и Обработка ---

    public bool CanAcceptCustomer()
    {
        return _isBuilt && _customerQueue.Count < buildingData.MaxQueueCapacity;
    }

    public void EnqueueCustomer(Customer customer)
    {
        _customerQueue.Enqueue(customer);
        UpdateQueuePositions();
        TryProcessNextCustomer();
    }

    private void TryProcessNextCustomer()
    {
        if (_customerQueue.Count == 0) return;

        // Ищем свободного работника
        WorkerPoint freePoint = GetFreeWorkerPoint();
        
        if (freePoint != null)
        {
            StartCoroutine(ProcessCustomerRoutine(freePoint));
        }
    }

    private WorkerPoint GetFreeWorkerPoint()
    {
        foreach (var wp in workerPoints)
        {
            if (wp.IsUnlocked && !wp.IsBusy) return wp;
        }
        return null;
    }

    private IEnumerator ProcessCustomerRoutine(WorkerPoint workerPoint)
    {
        workerPoint.SetBusy(true);
        OnStatsChanged?.Invoke();

        Customer currentCustomer = _customerQueue.Peek();
        
        // Логика прогресс-бара
        float timer = 0f;
        while (timer < _currentProcessingTime)
        {
            timer += Time.deltaTime;
            workerPoint.UpdateProgress(timer / _currentProcessingTime);
            yield return null;
        }

        // Завершение
        if (buildingData.ProfitPerCustomer > 0)
        {
            CurrencyController.Instance.AddCurrency(buildingData.ProfitPerCustomer);
        }

        currentCustomer.CompleteCurrentTask();
        _customerQueue.Dequeue();
        
        workerPoint.SetBusy(false);
        OnStatsChanged?.Invoke();
        
        UpdateQueuePositions();
        TryProcessNextCustomer(); 
    }

    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (var customer in _customerQueue)
        {
            Vector3 targetPos = queueStartPoint.position - (queueStartPoint.forward * index * 1.5f);
            customer.MoveToPosition(targetPos);
            index++;
        }
    }
}