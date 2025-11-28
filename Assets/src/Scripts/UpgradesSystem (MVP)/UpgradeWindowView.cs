using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeWindowView : MonoBehaviour, IView
{
    [Header("Main Info")]
    [SerializeField] private GameObject windowRoot;
    [SerializeField] private GameObject blackoutObject;
    
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI profitText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI workersText;

    [Header("Actions")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button speedUpgradeButton;
    [SerializeField] private TextMeshProUGUI speedCostText;
    [SerializeField] private Button workersUpgradeButton;
    [SerializeField] private TextMeshProUGUI workersCostText;

    public event Action OnCloseClicked;
    public event Action OnSpeedUpgradeClicked;
    public event Action OnWorkersUpgradeClicked;

    private void Start()
    {
        closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
        speedUpgradeButton.onClick.AddListener(() => OnSpeedUpgradeClicked?.Invoke());
        workersUpgradeButton.onClick.AddListener(() => OnWorkersUpgradeClicked?.Invoke());
    }

    public void Show()
    {
        windowRoot.SetActive(true);
        if (blackoutObject != null) blackoutObject.SetActive(true);
    }

    public void Hide()
    {
        windowRoot.SetActive(false);
        if (blackoutObject != null) blackoutObject.SetActive(false);
    }

    public void UpdateInfo(string name, int profit)
    {
        buildingNameText.text = name;
        profitText.text = $"Profit: {profit}$";
    }

    public void UpdateStats(float speed, int unlockedWorkers, int maxPossibleWorkers)
    {
        speedText.text = $"Speed: {speed:F2}s";
        workersText.text = $"Workers: {unlockedWorkers}/{maxPossibleWorkers}";
    }

    public void UpdateCosts(int speedCost, int workerCost, bool maxSpeedReached, bool maxWorkersReached, bool canAffordSpeed, bool canAffordWorkers)
    {
        // Speed
        if (maxSpeedReached)
        {
            speedCostText.text = "MAX";
            speedUpgradeButton.interactable = false;
        }
        else
        {
            speedCostText.text = $"{speedCost}$";
            speedUpgradeButton.interactable = canAffordSpeed;
        }

        // Workers
        if (maxWorkersReached)
        {
            workersCostText.text = "MAX";
            workersUpgradeButton.interactable = false;
        }
        else
        {
            workersCostText.text = $"{workerCost}$";
            workersUpgradeButton.interactable = canAffordWorkers;
        }
    }
}