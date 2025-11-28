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
        CurrencyController.Instance.AddCurrency(moneyAmount);
        _pulseAnimation.Play();
        
        // Добавляем звук вручную
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCoinSound(); 
    }
}