using UnityEngine;

/// <summary>
/// Manages special items (Golden Tickets) and drops.
/// </summary>
public class InventoryController : MonoBehaviour, ISaveable
{
    public static InventoryController Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int ticketRewardAmount = 250;
    [SerializeField] private float dropChance = 0.05f;
    [SerializeField] private TutorialData tutorialData;

    private int _ticketsCount = 0;
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
        GameEvents.InvokeTicketsChanged(_ticketsCount);
    }
    
    private void OnDestroy()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.UnregisterSaveable(this);
    }

    private void OnEnable() => GameEvents.OnSaleCompleted += HandleSaleCompleted;
    private void OnDisable() => GameEvents.OnSaleCompleted -= HandleSaleCompleted;

    private void HandleSaleCompleted()
    {
        if (TutorialController.Instance == null || !TutorialController.Instance.IsReadyForInventory) return;

        if (!_firstTicketDropped)
        {
            AddTicket(1);
            _firstTicketDropped = true;
            TutorialController.Instance.ShowHint(tutorialData.ticketFoundMessage, 0f);
            return;
        }

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

    public void UseTicket()
    {
        if (_ticketsCount <= 0) return;

        _ticketsCount--;
        CurrencyController.Instance.AddCurrency(ticketRewardAmount);
        GameEvents.InvokeTicketsChanged(_ticketsCount);

        if (!_firstTicketUsed)
        {
            _firstTicketUsed = true;
            TutorialController.Instance.ShowHint(tutorialData.ticketUsedMessage, 8f);
        }
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
}