using UnityEngine;

/// <summary>
/// Configuration for building stats and costs.
/// </summary>
[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Tycoon/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string buildingName;
    [SerializeField] private int buildCost;
    
    [Header("Economy")]
    [Tooltip("If > 0, generates money when customer is served.")]
    [SerializeField] private int profitPerCustomer;

    [Header("Processing Stats")]
    [SerializeField] private float baseProcessingTime = 2f;
    [SerializeField] private int baseWorkers = 1;
    [SerializeField] private int maxPossibleWorkers = 4; 
    [SerializeField] private int maxQueueCapacity = 5;

    [Header("Upgrades")]
    [SerializeField] private int speedUpgradeCost = 100;
    [SerializeField] private int maxSpeedUpgrades = 5;
    [SerializeField] private int workerUpgradeCost = 250;

    // Getters
    public string BuildingName => buildingName;
    public int BuildCost => buildCost;
    public int ProfitPerCustomer => profitPerCustomer;
    public float BaseProcessingTime => baseProcessingTime;
    public int BaseWorkers => baseWorkers;
    public int MaxPossibleWorkers => maxPossibleWorkers;
    public int MaxQueueCapacity => maxQueueCapacity;
    public int SpeedUpgradeCost => speedUpgradeCost;
    public int MaxSpeedUpgrades => maxSpeedUpgrades;
    public int WorkerUpgradeCost => workerUpgradeCost;
}