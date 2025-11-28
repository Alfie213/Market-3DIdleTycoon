using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public static TutorialController Instance { get; private set; }

    [SerializeField] private TutorialView view;

    public enum TutorialStep
    {
        BuildStalls = 0,
        WaitForCustomers = 1,
        FirstSaleMade = 2,  // Начиная с этого шага можно открывать окно
        UpgradeDone = 3,
        Completed = 4
    }

    private TutorialStep _currentStep = TutorialStep.BuildStalls;

    // Публичное свойство: Разрешены ли улучшения?
    // Разрешены, если мы дошли до шага FirstSaleMade ИЛИ закончили обучение.
    public bool IsUpgradesAllowed => _currentStep >= TutorialStep.FirstSaleMade;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ShowMessage("Welcome! Buy the Cashier and the Meat Stall to open your market.");
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

    private void HandleShopOpened()
    {
        if (_currentStep == TutorialStep.BuildStalls)
        {
            _currentStep = TutorialStep.WaitForCustomers;
            ShowMessage("Great job! The shop is OPEN. Customers are coming...");
        }
    }

    private void HandleSaleCompleted()
    {
        if (_currentStep == TutorialStep.WaitForCustomers)
        {
            _currentStep = TutorialStep.FirstSaleMade;
            ShowMessage("First profit made! Tap on a building and BUY an upgrade to boost efficiency!");
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
        view.ShowAndHideDelayed("You are a professional businessman now! Keep expanding to succeed.", 6f);
    }

    private void ShowMessage(string text)
    {
        view.Show(text);
    }
}