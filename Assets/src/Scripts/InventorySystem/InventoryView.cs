using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private GameObject contentRoot; // Весь объект кнопки
    [SerializeField] private Button ticketButton;
    [SerializeField] private TextMeshProUGUI countText;
    
    [Header("Animation")]
    [SerializeField] private PulseAnimation pulseAnimation; // Наш скрипт пульсации

    private void Start()
    {
        // Изначально скрыто
        contentRoot.SetActive(false);
        
        ticketButton.onClick.AddListener(OnTicketClicked);
    }

    private void OnEnable()
    {
        GameEvents.OnTicketsChanged += UpdateVisuals;
    }

    private void OnDisable()
    {
        GameEvents.OnTicketsChanged -= UpdateVisuals;
    }

    private void UpdateVisuals(int count)
    {
        // Если билеты появились (или это первый билет) - включаем UI
        // После первого включения UI больше не исчезает (согласно ТЗ), даже если count == 0
        // Но если вы хотите скрывать кнопку при 0, раскомментируйте else
        if (count > 0 && !contentRoot.activeSelf)
        {
            contentRoot.SetActive(true);
        }
        
        /*
        else if (count == 0)
        {
             // Если нужно скрывать кнопку, когда билетов нет
             // contentRoot.SetActive(false);
        }
        */

        countText.text = count.ToString();

        // Анимация при изменении (если добавился билет)
        if (pulseAnimation != null)
        {
            pulseAnimation.Play();
        }
    }

    private void OnTicketClicked()
    {
        InventoryController.Instance.UseTicket();
    }
}