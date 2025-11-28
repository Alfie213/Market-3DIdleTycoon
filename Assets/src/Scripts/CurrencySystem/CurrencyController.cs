using UnityEngine;

/// <summary>
/// Manages player's soft currency.
/// </summary>
public class CurrencyController : MonoBehaviour, ISaveable
{
    public static CurrencyController Instance { get; private set; }
    
    [SerializeField] private int startCurrency = 150;
    
    private int _currencyCount;
    public int CurrentCurrency => _currencyCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        _currencyCount = startCurrency;
    }

    private void Start()
    {
        SaveManager.Instance.RegisterSaveable(this);
        GameEvents.InvokeCurrencyChanged(_currencyCount);
    }

    private void OnDestroy()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.UnregisterSaveable(this);
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
        
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCoinSound();
    }

    public void PopulateSaveData(GameSaveData saveData) => saveData.money = _currencyCount;
    public void LoadFromSaveData(GameSaveData saveData)
    {
        _currencyCount = saveData.money;
        GameEvents.InvokeCurrencyChanged(_currencyCount);
    }
}