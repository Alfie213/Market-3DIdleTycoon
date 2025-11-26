using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private TutorialView view;

    private enum TutorialStep
    {
        BuildStalls,    // 1. Построй здания
        WaitForCustomers, // 2. Жди клиентов
        FirstSaleMade,  // 3. Клиент обслужен, нужно открыть окно апгрейда
        UpgradeDone,    // 4. Апгрейд куплен, финал
        Completed       // Обучение завершено
    }

    private TutorialStep _currentStep = TutorialStep.BuildStalls;

    private void Start()
    {
        // Шаг 1: Старт игры
        ShowMessage("Welcome! Buy the Cashier and the Vegetable Stall to open your market.");
    }

    private void OnEnable()
    {
        GameEvents.OnShopOpened += HandleShopOpened;
        GameEvents.OnCustomerServed += HandleCustomerServed;
        GameEvents.OnUpgradePurchased += HandleUpgradePurchased;
    }

    private void OnDisable()
    {
        GameEvents.OnShopOpened -= HandleShopOpened;
        GameEvents.OnCustomerServed -= HandleCustomerServed;
        GameEvents.OnUpgradePurchased -= HandleUpgradePurchased;
    }

    // Событие: Магазин открылся (построены оба здания)
    private void HandleShopOpened()
    {
        if (_currentStep == TutorialStep.BuildStalls)
        {
            _currentStep = TutorialStep.WaitForCustomers;
            ShowMessage("Great job! The shop is OPEN. Customers are coming...");
        }
    }

    // Событие: Кто-то купил товар
    private void HandleCustomerServed()
    {
        // Реагируем только если мы на шаге ожидания клиентов
        if (_currentStep == TutorialStep.WaitForCustomers)
        {
            _currentStep = TutorialStep.FirstSaleMade;
            ShowMessage("First profit! Tap on any building to open the Upgrade Menu.");
        }
    }

    // Событие: Игрок купил улучшение
    private void HandleUpgradePurchased()
    {
        // Реагируем только если мы на шаге "Сделай апгрейд"
        // (Или если мы еще на шаге WaitForCustomers, но игрок быстрый и успел проапгрейдить раньше времени - тоже засчитаем)
        if (_currentStep == TutorialStep.FirstSaleMade || _currentStep == TutorialStep.WaitForCustomers)
        {
            _currentStep = TutorialStep.UpgradeDone;
            FinishTutorial();
        }
    }

    private void FinishTutorial()
    {
        _currentStep = TutorialStep.Completed;
        // Показываем финал и скрываем через 5 секунд
        view.ShowAndHideDelayed("You are a professional businessman now! Keep expanding to succeed.", 6f);
        
        // Тут можно отключить сам объект контроллера, если он больше не нужен
        // Destroy(this); 
    }

    private void ShowMessage(string text)
    {
        view.Show(text);
    }
}