using UnityEngine;

public class CurrencyController : MonoBehaviour
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