using System.Collections;
using UnityEngine;

public class TutorialController : MonoBehaviour, ISaveable
{
    public static TutorialController Instance { get; private set; }

    [SerializeField] private TutorialView view;

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

        // Показываем дефолтное сообщение при старте.
        // Если сохранение загрузится, оно перезапишется в LoadFromSaveData.
        if (_currentStep == TutorialStep.BuildStalls)
        {
            ShowHint("Welcome! Buy the Cashier and the Meat Stall to open your market.", 0f);
        }
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.UnregisterSaveable(this);
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

    public void ShowHint(string text, float duration = 0f)
    {
        if (duration > 0)
            view.ShowAndHideDelayed(text, duration);
        else
            view.Show(text);
    }

    private void HandleShopOpened()
    {
        if (_currentStep == TutorialStep.BuildStalls)
        {
            _currentStep = TutorialStep.WaitForCustomers;
            ShowHint("Great job! The shop is OPEN. Customers are coming...", 0f);
        }
    }

    private void HandleSaleCompleted()
    {
        if (_currentStep == TutorialStep.WaitForCustomers)
        {
            _currentStep = TutorialStep.FirstSaleMade;
            ShowHint("First profit made! Tap on a building and BUY an upgrade to boost efficiency!", 0f);
        }
    }

    private void HandleUpgradePurchased()
    {
        if (_currentStep == TutorialStep.FirstSaleMade || _currentStep == TutorialStep.WaitForCustomers)
        {
            FinishTutorial();
        }
    }

    private void FinishTutorial()
    {
        _currentStep = TutorialStep.Completed;
        StartCoroutine(FinalSequenceRoutine());
    }

    private IEnumerator FinalSequenceRoutine()
    {
        float messageDuration = 12f;
        float delayAfterMessage = 5f;

        ShowHint("You are a professional businessman now! Remember: the more upgrades you buy, the more customers will come! Good luck!", messageDuration);

        yield return new WaitForSeconds(messageDuration);
        yield return new WaitForSeconds(delayAfterMessage);

        IsReadyForInventory = true;
    }

    // --- ISaveable Реализация ---

    public void PopulateSaveData(GameSaveData saveData)
    {
        saveData.tutorialStepIndex = (int)_currentStep;
        saveData.isTutorialInventoryReady = IsReadyForInventory;
    }

    public void LoadFromSaveData(GameSaveData saveData)
    {
        _currentStep = (TutorialStep)saveData.tutorialStepIndex;
        IsReadyForInventory = saveData.isTutorialInventoryReady;

        // --- ИСПРАВЛЕНИЕ ЗДЕСЬ ---
        // Восстанавливаем текст в зависимости от загруженного этапа
        switch (_currentStep)
        {
            case TutorialStep.BuildStalls:
                ShowHint("Welcome! Buy the Cashier and the Meat Stall to open your market.", 0f);
                break;

            case TutorialStep.WaitForCustomers:
                ShowHint("Great job! The shop is OPEN. Customers are coming...", 0f);
                break;

            case TutorialStep.FirstSaleMade:
                ShowHint("First profit made! Tap on a building and BUY an upgrade to boost efficiency!", 0f);
                break;

            case TutorialStep.UpgradeDone:
            case TutorialStep.Completed:
                // Если туториал пройден, скрываем окно (на случай, если оно было открыто дефолтным Start)
                view.Hide();
                break;
        }
        // -------------------------
    }
}