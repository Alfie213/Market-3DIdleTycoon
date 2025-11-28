using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the logic of a single building (Construction, Upgrades, Worker Assignment).
/// Acts as a central hub for interaction and visual state updates.
/// </summary>
public class BuildingController : MonoBehaviour, IInteractable, ISaveable
{
    [Header("Save System")]
    [Tooltip("Unique ID is required to identify this building in the save file.")]
    [SerializeField] private string buildingID;

    [Header("Data & Config")]
    [SerializeField] private BuildingData buildingData;
    
    [Header("Visual References")]
    [SerializeField] private GameObject visualModel;
    [SerializeField] private GameObject constructionSiteVisuals;
    [SerializeField] private ConstructionPriceDisplay priceDisplay; 
    
    [Header("Points")]
    [SerializeField] private Transform interactionPoint; 
    [SerializeField] private List<WorkerPoint> workerPoints; 

    /// <summary>
    /// Fired when any stat (workers count, speed, busy state) changes.
    /// Used by UI to update the view in real-time.
    /// </summary>
    public event Action OnStatsChanged;

    // Runtime state
    private bool _isBuilt = false;
    private int _currentUnlockedWorkers;
    private float _currentProcessingTime;
    private int _currentSpeedLevel = 0;

    #region Public Properties
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
    #endregion

    private void Awake()
    {
        // Initialize default stats from ScriptableObject
        _currentUnlockedWorkers = Mathf.Min(buildingData.BaseWorkers, buildingData.MaxPossibleWorkers);
        _currentProcessingTime = buildingData.BaseProcessingTime;
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(buildingID))
        {
            Debug.LogError($"Building {gameObject.name} has no ID!");
        }

        SaveManager.Instance.RegisterSaveable(this);

        if (priceDisplay != null) priceDisplay.SetPrice(buildingData.BuildCost);
        
        RefreshWorkerPoints();
        UpdateVisuals();
    }

    private void OnEnable()
    {
        // Subscribe to internal events
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

    /// <summary>
    /// Handles player input (Click on building).
    /// </summary>
    public void Interact()
    {
        if (!_isBuilt)
        {
            TryConstruct();
        }
        else
        {
            // Validation: Check if upgrades are allowed by Tutorial/Shop state
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
        // Reduce processing time by 10% per level
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

    /// <summary>
    /// Updates the state of individual WorkerPoints based on current upgrades.
    /// Passes new speed and profit data to workers.
    /// </summary>
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

    /// <summary>
    /// Checks if there is any free spot in queues for a new customer.
    /// </summary>
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

    /// <summary>
    /// Finds the worker with the shortest queue.
    /// </summary>
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

    #region ISaveable Implementation
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
            
            // Recalculate stats based on loaded levels
            _currentProcessingTime = buildingData.BaseProcessingTime * Mathf.Pow(0.9f, _currentSpeedLevel);
            _currentProcessingTime = Mathf.Max(0.1f, _currentProcessingTime);

            UpdateVisuals();
            RefreshWorkerPoints();
            
            if (_isBuilt) GameEvents.InvokeBuildingConstructed(this);
        }
    }
    #endregion
}