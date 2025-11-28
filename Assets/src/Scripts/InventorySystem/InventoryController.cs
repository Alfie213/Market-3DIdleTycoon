using UnityEngine;

public class InventoryController : MonoBehaviour
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

    private void OnEnable()
    {
        GameEvents.OnSaleCompleted += HandleSaleCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnSaleCompleted -= HandleSaleCompleted;
    }

    private void HandleSaleCompleted()
    {
        // Логика работает только если основной туториал завершен
        if (TutorialController.Instance == null || !TutorialController.Instance.IsTutorialCompleted) return;

        // 1. Сценарий первого билета
        if (!_firstTicketDropped)
        {
            AddTicket(1);
            _firstTicketDropped = true;
            
            // Показываем подсказку
            TutorialController.Instance.ShowHint("You found a GOLDEN TICKET! Tap on the ticket icon to claim your reward.");
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