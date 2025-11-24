using UnityEngine;

public enum BuildingType
{
    VegetableStall,
    Cashier
}

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Tycoon/Building Data")]
public class BuildingData : ScriptableObject
{
    [SerializeField] private string buildingName;
    [SerializeField] private BuildingType type;
    [SerializeField] private int cost;
    [SerializeField] private float processingTime;
    [SerializeField] private int maxQueueCapacity;
    [SerializeField] private int profitPerCustomer;

    public string BuildingName => buildingName;
    public BuildingType Type => type;
    public int Cost => cost;
    public float ProcessingTime => processingTime;
    public int MaxQueueCapacity => maxQueueCapacity;
    public int ProfitPerCustomer => profitPerCustomer;
}