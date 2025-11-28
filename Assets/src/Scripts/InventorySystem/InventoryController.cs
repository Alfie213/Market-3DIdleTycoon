using UnityEngine;

public class InventoryController : MonoBehaviour, ISaveable
{
    public static InventoryController Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int ticketRewardAmount = 250; // Награда за билет
    [SerializeField] private float dropChance = 0.05f; // 5% шанс выпадения

    private int _ticketsCount = 0;
    
    // Флаги для скриптованной сцены с первым билетом
    private bool _firstTicketDropped = false;
    private bool _firstTicketUsed = false;

    public int TicketsCount => _ticketsCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void Start()
    {
        SaveManager.Instance.RegisterSaveable(this);
        // Обновляем UI после регистрации, если загрузка произошла
        GameEvents.InvokeTicketsChanged(_ticketsCount);
    }

    private void OnEnable()
    {
        GameEvents.OnSaleCompleted += HandleSaleCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnSaleCompleted -= HandleSaleCompleted;
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.UnregisterSaveable(this);
    }
    
    public void PopulateSaveData(GameSaveData saveData)
    {
        saveData.tickets = _ticketsCount;
        saveData.isFirstTicketDropped = _firstTicketDropped;
        saveData.isFirstTicketUsed = _firstTicketUsed;
    }

    public void LoadFromSaveData(GameSaveData saveData)
    {
        _ticketsCount = saveData.tickets;
        _firstTicketDropped = saveData.isFirstTicketDropped;
        _firstTicketUsed = saveData.isFirstTicketUsed;
        
        GameEvents.InvokeTicketsChanged(_ticketsCount);
    }
    
    private void HandleSaleCompleted()
    {
        // --- ИЗМЕНЕНИЕ ЗДЕСЬ ---
        // Было: if (!TutorialController.Instance.IsTutorialCompleted)
        // Стало: Проверяем специальный флаг готовности
        if (TutorialController.Instance == null || !TutorialController.Instance.IsReadyForInventory) return;
        // -----------------------

        // 1. Сценарий первого билета
        if (!_firstTicketDropped)
        {
            // ... (код выдачи первого билета без изменений) ...
            AddTicket(1);
            _firstTicketDropped = true;
            TutorialController.Instance.ShowHint("You found a GOLDEN TICKET! Tap on the ticket icon to claim your reward.", 0f); // Этот хинт пусть висит пока не нажмут
            return;
        }

        // 2. Обычный рандом
        if (Random.value <= dropChance)
        {
            AddTicket(1);
        }
    }

    private void AddTicket(int amount)
    {
        _ticketsCount += amount;
        GameEvents.InvokeTicketsChanged(_ticketsCount);
    }

    // Метод вызывается при нажатии на UI кнопку билета
    public void UseTicket()
    {
        if (_ticketsCount <= 0) return;

        _ticketsCount--;
        CurrencyController.Instance.AddCurrency(ticketRewardAmount);
        GameEvents.InvokeTicketsChanged(_ticketsCount);

        // Реакция на использование ПЕРВОГО билета
        if (!_firstTicketUsed)
        {
            _firstTicketUsed = true;
            TutorialController.Instance.ShowHint(
                "The Golden Ticket gave you money! You can find them randomly by serving customers. Bigger shop = More customers = More tickets!", 
                8f // Показываем на 8 секунд и скрываем
            );
        }
    }
}