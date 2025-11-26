using UnityEngine;
using UnityEngine.UI;

public class WorldProgressBar : WorldSpaceBillboard // Наследуем
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject visuals;

    protected override void Awake()
    {
        base.Awake(); // Важно вызывать base.Awake(), чтобы найти камеру
        Hide();
    }
    
    // LateUpdate больше не нужен, он есть в родителе

    public void SetProgress(float value)
    {
        if (!visuals.activeSelf) visuals.SetActive(true);
        slider.value = value;
    }

    public void Hide()
    {
        visuals.SetActive(false);
    }
}