using UnityEngine;

/// <summary>
/// Interactive object giving free currency.
/// </summary>
[RequireComponent(typeof(PulseAnimation))]
public class ATM : MonoBehaviour, IInteractable
{
    [SerializeField] private int moneyAmount = 10;
    private PulseAnimation _pulseAnimation;

    private void Awake() => _pulseAnimation = GetComponent<PulseAnimation>();

    public void Interact()
    {
        CurrencyController.Instance.AddCurrency(moneyAmount);
        _pulseAnimation.Play();
    }
}