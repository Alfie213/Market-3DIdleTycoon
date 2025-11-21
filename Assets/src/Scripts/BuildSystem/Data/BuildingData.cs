using UnityEngine;

public class BuildingData : ScriptableObject
{
    public int cost;
    
    public int maxWorkersCount;
    public int costPerWorker;

    public float baseProcessingTime;
    public int costPerSpeedUpgrade;
}
