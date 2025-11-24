using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject upgradeWindow;
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI statsText; // Показывает текущую скор. и работников
    
    [Header("Buttons")]
    [SerializeField] private Button speedUpgradeBtn;
    [SerializeField] private TextMeshProUGUI speedCostText;
    
    [SerializeField] private Button workersUpgradeBtn;
    [SerializeField] private TextMeshProUGUI workersCostText;
    
    [SerializeField] private Button closeBtn;

    private BuildingController _currentBuilding;

    private void Start()
    {
        upgradeWindow.SetActive(false);
        
        speedUpgradeBtn.onClick.AddListener(TryUpgradeSpeed);
        workersUpgradeBtn.onClick.AddListener(TryUpgradeWorkers);
        closeBtn.onClick.AddListener(CloseWindow);
    }

    private void OnEnable()
    {
        GameEvents.OnUpgradeWindowRequested += OpenWindow;
    }

    private void OnDisable()
    {
        GameEvents.OnUpgradeWindowRequested -= OpenWindow;
    }

    private void OpenWindow(BuildingController building)
    {
        _currentBuilding = building;
        UpdateUI();
        upgradeWindow.SetActive(true);
    }

    private void CloseWindow()
    {
        _currentBuilding = null;
        upgradeWindow.SetActive(false);
    }

    private void UpdateUI()
    {
        if (_currentBuilding == null) return;

        buildingNameText.text = _currentBuilding.Data.BuildingName;
        
        // Отображение статов
        statsText.text = $"Speed: {_currentBuilding.CurrentProcessingTime:F1}s | Workers: {_currentBuilding.CurrentMaxWorkers}";

        // Цены
        speedCostText.text = $"{_currentBuilding.Data.SpeedUpgradeCost}$";
        workersCostText.text = $"{_currentBuilding.Data.WorkerUpgradeCost}$";
    }

    private void TryUpgradeSpeed()
    {
        if (_currentBuilding == null) return;

        int cost = _currentBuilding.Data.SpeedUpgradeCost;
        if (CurrencyController.Instance.TrySpendCurrency(cost))
        {
            _currentBuilding.UpgradeSpeed();
            UpdateUI();
        }
    }

    private void TryUpgradeWorkers()
    {
        if (_currentBuilding == null) return;

        int cost = _currentBuilding.Data.WorkerUpgradeCost;
        if (CurrencyController.Instance.TrySpendCurrency(cost))
        {
            _currentBuilding.UpgradeWorkers();
            UpdateUI();
        }
    }
}