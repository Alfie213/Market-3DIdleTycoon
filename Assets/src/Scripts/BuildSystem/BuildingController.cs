using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour, IInteractable
{
    [SerializeField] private BuildingData buildingData;
    
    [Header("Visuals")]
    [SerializeField] private GameObject visualModel;
    [SerializeField] private GameObject constructionSiteVisuals;
    [SerializeField] private ConstructionPriceDisplay priceDisplay; 
    
    [Header("Interaction")]
    [SerializeField] private Transform interactionPoint; 
    
    [Header("Workers")]
    [SerializeField] private List<WorkerPoint> workerPoints; 

    public event Action OnStatsChanged;

    private bool _isBuilt = false;
    private int _currentUnlockedWorkers;
    private float _currentProcessingTime;
    
    private int _currentSpeedLevel = 0;
    
    public int CurrentSpeedLevel => _currentSpeedLevel;

    // Свойства...
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
        
        foreach (var wp in workerPoints)
        {
            wp.OnStateChanged += () => OnStatsChanged?.Invoke();
        }

        UpdateVisuals();
    }

    private void Start()
    {
        if (priceDisplay != null)
        {
            priceDisplay.SetPrice(buildingData.BuildCost);
            // ПОДПИСКА НА КНОПКУ
            priceDisplay.OnBuyClicked += TryConstruct;
        }

        RefreshWorkerPoints();
    }

    private void OnDestroy()
    {
        // ОТПИСКА (Хороший тон, чтобы избежать утечек памяти)
        if (priceDisplay != null)
        {
            priceDisplay.OnBuyClicked -= TryConstruct;
        }
    }

    public void Interact()
    {
        if (!_isBuilt)
        {
            TryConstruct();
        }
        else
        {
            // 1. Проверка магазина (уже была)
            if (ShopController.Instance != null && !ShopController.Instance.IsShopOpen)
            {
                // Можно добавить визуальный фидбек (звук ошибки или всплывающий текст)
                return;
            }

            // 2. НОВАЯ ПРОВЕРКА ТУТОРИАЛА
            if (TutorialController.Instance != null && !TutorialController.Instance.IsUpgradesAllowed)
            {
                // Игрок пытается нажать раньше времени.
                // Можно вывести лог или подсказку "Wait for customers!"
                Debug.Log("Wait for the tutorial instruction!");
                return;
            }
            
            GameEvents.InvokeUpgradeWindowRequested(this);
        }
    }

    // Этот метод теперь вызывается и при клике по 3D модели, и при клике по кнопке
    private void TryConstruct()
    {
        if (_isBuilt) return; // Защита от двойного нажатия

        if (CurrencyController.Instance.TrySpendCurrency(buildingData.BuildCost))
        {
            _isBuilt = true;
            UpdateVisuals();
            RefreshWorkerPoints(); 
            GameEvents.InvokeBuildingConstructed(this);
        }
    }

    // ... (Остальные методы: UpgradeSpeed, UpgradeWorkers, RefreshWorkerPoints и т.д. без изменений) ...
    public void UpgradeSpeed()
    {
        // Проверка лимита
        if (_currentSpeedLevel >= buildingData.MaxSpeedUpgrades) return;

        _currentSpeedLevel++; // Увеличиваем уровень
        
        _currentProcessingTime = Mathf.Max(0.1f, _currentProcessingTime * 0.9f);
        RefreshWorkerPoints();
        OnStatsChanged?.Invoke();
        
        GameEvents.InvokeUpgradePurchased();
    }

    public void UpgradeWorkers()
    {
        if (_currentUnlockedWorkers >= buildingData.MaxPossibleWorkers) return;
        _currentUnlockedWorkers++;
        RefreshWorkerPoints();
        OnStatsChanged?.Invoke();
        
        GameEvents.InvokeUpgradePurchased();
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
            workerPoints[i].Initialize(_currentProcessingTime, buildingData.ProfitPerCustomer);
            workerPoints[i].SetUnlocked(shouldBeActive);
        }
    }

    private void UpdateVisuals()
    {
        if (visualModel) visualModel.SetActive(_isBuilt);
        if (constructionSiteVisuals) constructionSiteVisuals.SetActive(!_isBuilt);

        if (priceDisplay != null)
        {
            if (_isBuilt) priceDisplay.Hide();
            else priceDisplay.Show();
        }
    }
    
    // ... CanAcceptCustomer, EnqueueCustomer, GetBestWorkerPoint без изменений ...
    public bool CanAcceptCustomer()
    {
        if (!_isBuilt) return false;
        foreach (var wp in workerPoints)
        {
            if (wp.IsUnlocked && wp.QueueCount < buildingData.MaxQueueCapacity) return true;
        }
        return false;
    }

    public void EnqueueCustomer(Customer customer)
    {
        WorkerPoint bestPoint = GetBestWorkerPoint();
        if (bestPoint != null) bestPoint.EnqueueCustomer(customer);
    }

    private WorkerPoint GetBestWorkerPoint()
    {
        WorkerPoint bestPoint = null;
        int minQueue = int.MaxValue;

        foreach (var wp in workerPoints)
        {
            if (!wp.IsUnlocked) continue;
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