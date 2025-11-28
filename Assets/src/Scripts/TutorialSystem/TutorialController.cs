using UnityEngine;

public class TutorialController : MonoBehaviour
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

    public bool IsTutorialCompleted => _currentStep == TutorialStep.Completed;
    public bool IsUpgradesAllowed => _currentStep >= TutorialStep.FirstSaleMade;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 0 = бесконечно (пока не выполнит задание)
        ShowHint("Welcome! Buy the Cashier and the Meat Stall to open your market.", 0f);
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

    // --- УНИВЕРСАЛЬНЫЙ МЕТОД ---
    // Мы удалили ShowMessage и заменили его на ShowHint.
    // duration: 0 - висит бесконечно, > 0 - исчезает через время.
    public void ShowHint(string text, float duration = 0f)
    {
        if (duration > 0)
        {
            view.ShowAndHideDelayed(text, duration);
        }
        else
        {
            view.Show(text);
        }
    }
    // ---------------------------

    private void HandleShopOpened()
    {
        if (_currentStep == TutorialStep.BuildStalls)
        {
            _currentStep = TutorialStep.WaitForCustomers;
            // Важная инструкция - висит бесконечно (0)
            ShowHint("Great job! The shop is OPEN. Customers are coming...", 0f);
        }
    }

    private void HandleSaleCompleted()
    {
        if (_currentStep == TutorialStep.WaitForCustomers)
        {
            _currentStep = TutorialStep.FirstSaleMade;
            // Важная инструкция - висит бесконечно (0)
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
        // Финальное сообщение - можно скрыть через 12 секунд
        ShowHint("You are a professional businessman now! Remember: the more upgrades you buy, the more customers will come! Good luck!", 12f);
    }
}