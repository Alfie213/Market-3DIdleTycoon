using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Tycoon/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string buildingName;
    [SerializeField] private int buildCost;
    
    [Header("Economy")]
    [SerializeField] private int profitPerCustomer;

    [Header("Processing Stats")]
    [SerializeField] private float baseProcessingTime = 2f;
    [SerializeField] private int baseWorkers = 1;
    [Tooltip("Абсолютный лимит работников для этого типа здания (сколько точек расставлено)")]
    [SerializeField] private int maxPossibleWorkers = 4; 
    [SerializeField] private int maxQueueCapacity = 5;

    [Header("Upgrade Costs")]
    [SerializeField] private int speedUpgradeCost = 100;
    [SerializeField] private int workerUpgradeCost = 250;

    public string BuildingName => buildingName;
    public int BuildCost => buildCost;
    public int ProfitPerCustomer => profitPerCustomer;
    public float BaseProcessingTime => baseProcessingTime;
    public int BaseWorkers => baseWorkers;
    public int MaxPossibleWorkers => maxPossibleWorkers; // Новое поле
    public int MaxQueueCapacity => maxQueueCapacity;
    public int SpeedUpgradeCost => speedUpgradeCost;
    public int WorkerUpgradeCost => workerUpgradeCost;
}