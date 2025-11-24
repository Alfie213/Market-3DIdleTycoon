using TMPro;
using UnityEngine;

public class CurrencyObserver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyCurrentAmountTMP;

    private void OnEnable()
    {
        GameEvents.OnCurrencyChanged += EditCurrencyCount;
    }

    private void OnDisable()
    {
        GameEvents.OnCurrencyChanged -= EditCurrencyCount;
    }

    private void EditCurrencyCount(int currentAmount)
    {
        currencyCurrentAmountTMP.text = currentAmount.ToString();
    }
}
