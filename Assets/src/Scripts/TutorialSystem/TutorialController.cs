using System.Collections;
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
    
    // Новое свойство: Готовы ли мы к выдаче билетов?
    // По умолчанию false, станет true только спустя 5 секунд ПОСЛЕ исчезновения текста
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
        // 1. Помечаем, что механика туториала пройдена (можно строить, играть)
        _currentStep = TutorialStep.Completed;
        
        // 2. Запускаем финальную последовательность таймеров
        StartCoroutine(FinalSequenceRoutine());
    }

    private IEnumerator FinalSequenceRoutine()
    {
        float messageDuration = 12f;
        float delayAfterMessage = 5f;

        // Показываем текст
        ShowHint("You are a professional businessman now! Remember: the more upgrades you buy, the more customers will come! Good luck!", messageDuration);

        // Ждем пока текст висит (12 сек)
        yield return new WaitForSeconds(messageDuration);
        
        // Текст исчез (View скроет его сама по таймеру).
        // Теперь ждем 5 секунд тишины.
        yield return new WaitForSeconds(delayAfterMessage);

        // Включаем доступ к инвентарю
        IsReadyForInventory = true;
    }
}