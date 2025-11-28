using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Core logic for a building. Handles construction, upgrades, and worker management.
/// </summary>
public class BuildingController : MonoBehaviour, IInteractable, ISaveable
{
    [Header("Identity & Data")]
    [SerializeField] private string buildingID;
    [SerializeField] private BuildingData buildingData;
    
    [Header("Visuals")]
    [SerializeField] private GameObject visualModel;
    [SerializeField] private GameObject constructionSiteVisuals;
    [SerializeField] private ConstructionPriceDisplay priceDisplay; 
    
    [Header("Setup")]
    [SerializeField] private Transform interactionPoint; 
    [SerializeField] private List<WorkerPoint> workerPoints; 

    public event Action OnStatsChanged;

    private bool _isBuilt = false;
    private int _currentUnlockedWorkers;
    private float _currentProcessingTime;
    private int _currentSpeedLevel = 0;

    public BuildingData Data => buildingData;
    public bool IsBuilt => _isBuilt;
    public Transform InteractionPoint => interactionPoint;
    public int CurrentUnlockedWorkers => _currentUnlockedWorkers;
    public int MaxPossibleWorkers => buildingData.MaxPossibleWorkers;
    public float CurrentProcessingTime => _currentProcessingTime;
    public int CurrentSpeedLevel => _currentSpeedLevel;
    
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
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(buildingID)) Debug.LogError($"Building {gameObject.name} has no ID!");

        SaveManager.Instance.RegisterSaveable(this);

        if (priceDisplay != null) priceDisplay.SetPrice(buildingData.BuildCost);
        
        RefreshWorkerPoints();
        UpdateVisuals();
    }

    private void OnEnable()
    {
        foreach (var wp in workerPoints) wp.OnStateChanged += HandleWorkerStateChanged;
        if (priceDisplay != null) priceDisplay.OnBuyClicked += TryConstruct;
    }

    private void OnDisable()
    {
        foreach (var wp in workerPoints) wp.OnStateChanged -= HandleWorkerStateChanged;
        if (priceDisplay != null) priceDisplay.OnBuyClicked -= TryConstruct;
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.UnregisterSaveable(this);
    }

    private void HandleWorkerStateChanged() => OnStatsChanged?.Invoke();

    public void Interact()
    {
        if (!_isBuilt)
        {
            TryConstruct();
        }
        else
        {
            if (ShopController.Instance != null && !ShopController.Instance.IsShopOpen) return;
            if (TutorialController.Instance != null && !TutorialController.Instance.IsUpgradesAllowed) return;
            
            GameEvents.InvokeUpgradeWindowRequested(this);
        }
    }

    private void TryConstruct()
    {
        if (_isBuilt) return;

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
        if (_currentSpeedLevel >= buildingData.MaxSpeedUpgrades) return;

        _currentSpeedLevel++;
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

    public void PopulateSaveData(GameSaveData saveData)
    {
        BuildingSaveData data = saveData.buildings.Find(b => b.id == buildingID);
        if (data == null)
        {
            data = new BuildingSaveData { id = buildingID };
            saveData.buildings.Add(data);
        }

        data.isBuilt = _isBuilt;
        data.speedLevel = _currentSpeedLevel;
        data.unlockedWorkers = _currentUnlockedWorkers;
    }

    public void LoadFromSaveData(GameSaveData saveData)
    {
        BuildingSaveData data = saveData.buildings.Find(b => b.id == buildingID);
        if (data != null)
        {
            _isBuilt = data.isBuilt;
            _currentSpeedLevel = data.speedLevel;
            _currentUnlockedWorkers = data.unlockedWorkers;
            
            _currentProcessingTime = buildingData.BaseProcessingTime * Mathf.Pow(0.9f, _currentSpeedLevel);
            _currentProcessingTime = Mathf.Max(0.1f, _currentProcessingTime);

            UpdateVisuals();
            RefreshWorkerPoints();
            
            if (_isBuilt) GameEvents.InvokeBuildingConstructed(this);
        }
    }
}