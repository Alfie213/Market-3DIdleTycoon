using UnityEngine;

public abstract class BuildingObjectBase : MonoBehaviour
{
    [SerializeField] private BuildingData currentBuildingData;
    [SerializeField] private ConstructionCostPopUp constructionCostPopUp;
}
