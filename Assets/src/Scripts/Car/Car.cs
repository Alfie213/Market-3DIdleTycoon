using UnityEngine;

[RequireComponent(typeof(PulseAnimation))]
public class Car : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip honkSound;
    
    private PulseAnimation _pulseAnimation;

    private void Awake()
    {
        _pulseAnimation = GetComponent<PulseAnimation>();
    }

    public void Interact()
    {
        _pulseAnimation.Play();

        if (AudioManager.Instance != null && honkSound != null)
        {
            AudioManager.Instance.PlaySFX(honkSound);
        }
    }
}