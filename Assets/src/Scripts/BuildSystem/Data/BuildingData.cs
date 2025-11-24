using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Tycoon/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("General")]
    [SerializeField] private string buildingName;
    [SerializeField] private int buildCost;
    
    [Header("Economy")]
    [Tooltip("Если 0, то здание не приносит денег (овощная лавка). Если > 0, приносит (касса).")]
    [SerializeField] private int profitPerCustomer;

    [Header("Processing Stats (Base)")]
    [SerializeField] private float baseProcessingTime = 2f;
    [SerializeField] private int baseMaxWorkers = 1;
    [SerializeField] private int maxQueueCapacity = 5;

    [Header("Upgrade Costs")]
    [SerializeField] private int speedUpgradeCost = 100;
    [SerializeField] private int workerUpgradeCost = 250;

    // Getters
    public string BuildingName => buildingName;
    public int BuildCost => buildCost;
    public int ProfitPerCustomer => profitPerCustomer;
    public float BaseProcessingTime => baseProcessingTime;
    public int BaseMaxWorkers => baseMaxWorkers;
    public int MaxQueueCapacity => maxQueueCapacity;
    public int SpeedUpgradeCost => speedUpgradeCost;
    public int WorkerUpgradeCost => workerUpgradeCost;
}