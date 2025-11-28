using TMPro;
using UnityEngine;

public class CurrencyObserver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] currencyTexts;
    private PulseAnimation[] _pulseAnimations;

    private void Awake()
    {
        if (currencyTexts == null) return;
        _pulseAnimations = new PulseAnimation[currencyTexts.Length];
        for (int i = 0; i < currencyTexts.Length; i++)
        {
            if (currencyTexts[i] != null) _pulseAnimations[i] = currencyTexts[i].GetComponent<PulseAnimation>();
        }
    }

    private void Start()
    {
        if (CurrencyController.Instance != null) UpdateVisuals(CurrencyController.Instance.CurrentCurrency);
    }

    private void OnEnable() => GameEvents.OnCurrencyChanged += UpdateVisuals;
    private void OnDisable() => GameEvents.OnCurrencyChanged -= UpdateVisuals;

    private void UpdateVisuals(int amount)
    {
        if (currencyTexts == null) return;
        for (int i = 0; i < currencyTexts.Length; i++)
        {
            if (currencyTexts[i] != null)
            {
                currencyTexts[i].text = $"{amount}$";
                if (_pulseAnimations != null && i < _pulseAnimations.Length && _pulseAnimations[i] != null)
                {
                    _pulseAnimations[i].Play();
                }
            }
        }
    }
}