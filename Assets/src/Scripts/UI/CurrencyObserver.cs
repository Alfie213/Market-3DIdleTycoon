using TMPro;
using UnityEngine;

public class CurrencyObserver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] currencyTexts;
    
    private UIPulseAnimation[] _pulseAnimations;

    // Инициализацию ссылок делаем в Awake, чтобы они были готовы до включения событий
    private void Awake()
    {
        // Защита от дурака, если забыли назначить тексты в инспекторе
        if (currencyTexts == null) return;

        _pulseAnimations = new UIPulseAnimation[currencyTexts.Length];

        for (int i = 0; i < currencyTexts.Length; i++)
        {
            if (currencyTexts[i] != null)
            {
                _pulseAnimations[i] = currencyTexts[i].GetComponent<UIPulseAnimation>();
            }
        }
    }

    private void Start()
    {
        // Обновляем визуальное состояние при старте
        if (CurrencyController.Instance != null)
        {
            UpdateVisuals(CurrencyController.Instance.CurrentCurrency);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnCurrencyChanged += UpdateVisuals;
    }

    private void OnDisable()
    {
        GameEvents.OnCurrencyChanged -= UpdateVisuals;
    }

    private void UpdateVisuals(int amount)
    {
        if (currencyTexts == null) return;

        for (int i = 0; i < currencyTexts.Length; i++)
        {
            if (currencyTexts[i] != null)
            {
                currencyTexts[i].text = $"{amount}$";

                // Добавляем проверку, инициализирован ли массив (на всякий случай)
                // и есть ли анимация
                if (_pulseAnimations != null && i < _pulseAnimations.Length && _pulseAnimations[i] != null)
                {
                    _pulseAnimations[i].Play();
                }
            }
        }
    }
}