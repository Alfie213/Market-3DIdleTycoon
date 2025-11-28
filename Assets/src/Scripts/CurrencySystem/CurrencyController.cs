using UnityEngine;

public class CurrencyController : MonoBehaviour, ISaveable
{
    public static CurrencyController Instance { get; private set; }
    
    [SerializeField] private int startCurrency = 150;
    
    private int _currencyCount;

    // --- ДОБАВЛЕНО ---
    public int CurrentCurrency => _currencyCount;
    // -----------------

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _currencyCount = startCurrency;
    }

    private void Start()
    {
        SaveManager.Instance.RegisterSaveable(this); // Регистрируемся
        
        // ВАЖНО: Мы не вызываем LoadGame() здесь.
        // SaveManager сам вызовет LoadFromSaveData, когда будет нужно.
        // Но для начального состояния валюты мы отправляем ивент.
        GameEvents.InvokeCurrencyChanged(_currencyCount); 
    }
    
    private void OnDestroy()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.UnregisterSaveable(this);
    }

    // --- РЕАЛИЗАЦИЯ ИНТЕРФЕЙСА ---

    public void PopulateSaveData(GameSaveData saveData)
    {
        saveData.money = _currencyCount;
    }

    public void LoadFromSaveData(GameSaveData saveData)
    {
        _currencyCount = saveData.money;
        GameEvents.InvokeCurrencyChanged(_currencyCount);
    }

    public bool TrySpendCurrency(int amount)
    {
        if (amount < 0 || _currencyCount < amount) return false;
        
        _currencyCount -= amount;
        GameEvents.InvokeCurrencyChanged(_currencyCount);
        return true;
    }

    public void AddCurrency(int amount)
    {
        if (amount <= 0) return;
        
        _currencyCount += amount;
        GameEvents.InvokeCurrencyChanged(_currencyCount);
    }
}