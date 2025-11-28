using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the game onboarding flow.
/// Shows hints based on game events (Sales, Building, etc.).
/// </summary>
public class TutorialController : MonoBehaviour, ISaveable
{
    public static TutorialController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TutorialView view;
    [SerializeField] private TutorialData data; // <-- Ссылка на данные с текстом

    public enum TutorialStep
    {
        BuildStalls = 0,
        WaitForCustomers = 1,
        FirstSaleMade = 2,
        UpgradeDone = 3,
        Completed = 4
    }

    private TutorialStep _currentStep = TutorialStep.BuildStalls;
    
    public bool IsReadyForInventory { get; private set; } = false;
    public bool IsTutorialCompleted => _currentStep == TutorialStep.Completed;
    public bool IsUpgradesAllowed => _currentStep >= TutorialStep.FirstSaleMade;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SaveManager.Instance.RegisterSaveable(this);

        if (_currentStep == TutorialStep.BuildStalls)
        {
            // Используем текст из Data
            ShowHint(data.welcomeMessage, 0f);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnShopOpened += HandleShopOpened;
        GameEvents.OnSaleCompleted += HandleSaleCompleted;
        GameEvents.OnUpgradePurchased += HandleUpgradePurchased;
    }

    private void OnDisable()
    {
        GameEvents.OnShopOpened -= HandleShopOpened;
        GameEvents.OnSaleCompleted -= HandleSaleCompleted;
        GameEvents.OnUpgradePurchased -= HandleUpgradePurchased;
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.UnregisterSaveable(this);
    }

    public void ShowHint(string text, float duration = 0f)
    {
        if (duration > 0)
            view.ShowAndHideDelayed(text, duration);
        else
            view.Show(text);
    }

    #region Event Handlers
    private void HandleShopOpened()
    {
        if (_currentStep == TutorialStep.BuildStalls)
        {
            _currentStep = TutorialStep.WaitForCustomers;
            ShowHint(data.shopOpenedMessage, 0f); // <-- Текст из Data
        }
    }

    private void HandleSaleCompleted()
    {
        if (_currentStep == TutorialStep.WaitForCustomers)
        {
            _currentStep = TutorialStep.FirstSaleMade;
            ShowHint(data.firstProfitMessage, 0f); // <-- Текст из Data
        }
    }

    private void HandleUpgradePurchased()
    {
        if (_currentStep == TutorialStep.FirstSaleMade || _currentStep == TutorialStep.WaitForCustomers)
        {
            FinishTutorial();
        }
    }
    #endregion

    private void FinishTutorial()
    {
        _currentStep = TutorialStep.Completed;
        StartCoroutine(FinalSequenceRoutine());
    }

    private IEnumerator FinalSequenceRoutine()
    {
        float messageDuration = 12f;
        float delayAfterMessage = 5f;

        ShowHint(data.completionMessage, messageDuration); // <-- Текст из Data

        yield return new WaitForSeconds(messageDuration);
        yield return new WaitForSeconds(delayAfterMessage);

        IsReadyForInventory = true;
    }

    #region ISaveable
    public void PopulateSaveData(GameSaveData saveData)
    {
        saveData.tutorialStepIndex = (int)_currentStep;
        saveData.isTutorialInventoryReady = IsReadyForInventory;
    }

    public void LoadFromSaveData(GameSaveData saveData)
    {
        _currentStep = (TutorialStep)saveData.tutorialStepIndex;
        IsReadyForInventory = saveData.isTutorialInventoryReady;

        // Восстанавливаем текст, используя данные из ScriptableObject
        switch (_currentStep)
        {
            case TutorialStep.BuildStalls:
                ShowHint(data.welcomeMessage, 0f);
                break;
            case TutorialStep.WaitForCustomers:
                ShowHint(data.shopOpenedMessage, 0f);
                break;
            case TutorialStep.FirstSaleMade:
                ShowHint(data.firstProfitMessage, 0f);
                break;
            case TutorialStep.UpgradeDone:
            case TutorialStep.Completed:
                view.Hide();
                break;
        }
    }
    #endregion
}