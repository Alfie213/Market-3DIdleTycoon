using UnityEngine;

[RequireComponent(typeof(PulseAnimation))]
public class ATM : MonoBehaviour, IInteractable
{
    [SerializeField] private int moneyAmount = 10;
    
    private PulseAnimation _pulseAnimation;

    private void Awake()
    {
        _pulseAnimation = GetComponent<PulseAnimation>();
    }

    // Реализация интерфейса
    public void Interact()
    {
        // 1. Даем деньги
        CurrencyController.Instance.AddCurrency(moneyAmount);
        
        // 2. Анимируем
        _pulseAnimation.Play();
        
        // Опционально: можно добавить звук или партиклы
        Debug.Log($"ATM gave {moneyAmount}$");
    }
}