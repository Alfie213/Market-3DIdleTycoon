using UnityEngine;

public class BuildingData : ScriptableObject
{
    public int cost;
    public GameObject prefab;
    public BuildingData upgradeTo;

    // public int baseIncome;
    // public float incomeInterval;
}
