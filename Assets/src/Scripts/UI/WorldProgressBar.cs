using UnityEngine;
using UnityEngine.UI;

public class WorldProgressBar : WorldSpaceBillboard
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject visuals;

    protected override void Awake()
    {
        base.Awake();
        Hide();
    }

    public void SetProgress(float value)
    {
        if (!visuals.activeSelf) visuals.SetActive(true);
        slider.value = value;
    }

    public void Hide() => visuals.SetActive(false);
}