using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private Button ticketButton;
    [SerializeField] private TextMeshProUGUI countText;
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
        if (count > 0 && !contentRoot.activeSelf) contentRoot.SetActive(true);
        countText.text = count.ToString();
        if (pulseAnimation != null) pulseAnimation.Play();
    }
}