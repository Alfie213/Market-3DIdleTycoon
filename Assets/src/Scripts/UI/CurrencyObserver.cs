using TMPro;
using UnityEngine;

public class CurrencyObserver : MonoBehaviour
{
    // Массив для привязки нескольких текстовых полей (HUD, Магазин, Меню паузы и т.д.)
    [SerializeField] private TextMeshProUGUI[] currencyTexts;

    private void Start()
    {
        // При старте сразу показываем актуальное значение, если контроллер уже существует
        if (CurrencyController.Instance != null)
        {
            UpdateVisuals(CurrencyController.Instance.CurrentCurrency);
        }
    }

    private void OnEnable()
    {
        // Подписываемся на глобальное событие
        GameEvents.OnCurrencyChanged += UpdateVisuals;
    }

    private void OnDisable()
    {
        GameEvents.OnCurrencyChanged -= UpdateVisuals;
    }

    private void UpdateVisuals(int amount)
    {
        // Проходимся по всем привязанным текстам
        foreach (var textMesh in currencyTexts)
        {
            if (textMesh != null)
            {
                // Интерполяция строки: Значение + Знак доллара
                textMesh.text = $"{amount}$";
            }
        }
    }
}