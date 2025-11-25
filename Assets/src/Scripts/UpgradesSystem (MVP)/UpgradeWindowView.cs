using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeWindowView : MonoBehaviour
{
    [Header("Main Info")]
    [SerializeField] private GameObject windowRoot;
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI profitText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI workersText; // "1/4"

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

    public void SetWindowActive(bool isActive)
    {
        windowRoot.SetActive(isActive);
    }

    public void UpdateInfo(string name, int profit)
    {
        buildingNameText.text = name;
        profitText.text = $"Profit: {profit}$";
    }

    public void UpdateStats(float speed, int unlockedWorkers, int maxPossibleWorkers)
    {
        speedText.text = $"Speed: {speed:F2}s";
        // Теперь выводит: 1 / 4 (где 1 - текущий уровень прокачки, 4 - лимит)
        workersText.text = $"Workers: {unlockedWorkers}/{maxPossibleWorkers}";
    }

    public void UpdateCosts(int speedCost, int workerCost, bool maxWorkersReached)
    {
        speedCostText.text = $"{speedCost}$";
        
        if (maxWorkersReached)
        {
            workersCostText.text = "MAX";
            workersUpgradeButton.interactable = false;
        }
        else
        {
            workersCostText.text = $"{workerCost}$";
            workersUpgradeButton.interactable = true;
        }
    }
}