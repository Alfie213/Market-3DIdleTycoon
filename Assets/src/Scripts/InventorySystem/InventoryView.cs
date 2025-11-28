using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private Button ticketButton;
    [SerializeField] private TextMeshProUGUI countText;
    
    [Header("Animation")]
    [SerializeField] private PulseAnimation pulseAnimation;

    private void Start()
    {
        contentRoot.SetActive(false);
        ticketButton.onClick.AddListener(() => InventoryController.Instance.UseTicket());
    }

    private void OnEnable() => GameEvents.OnTicketsChanged += UpdateVisuals;
    private void OnDisable() => GameEvents.OnTicketsChanged -= UpdateVisuals;

    private void UpdateVisuals(int count)
    {
        bool shouldBeVisible = count > 0;

        if (contentRoot.activeSelf != shouldBeVisible)
        {
            contentRoot.SetActive(shouldBeVisible);
        }

        if (shouldBeVisible)
        {
            countText.text = count.ToString();
            if (pulseAnimation != null) pulseAnimation.Play();
        }
    }
}