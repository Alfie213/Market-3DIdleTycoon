using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionPriceDisplay : WorldSpaceBillboard
{
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject contentRoot; 

    public event Action OnBuyClicked;

    protected override void Awake()
    {
        base.Awake();
        if (buyButton != null) buyButton.onClick.AddListener(() => OnBuyClicked?.Invoke());
    }

    public void SetPrice(int price)
    {
        if (priceText != null) priceText.text = $"{price}$";
    }

    public void Show() => contentRoot.SetActive(true);
    public void Hide() => contentRoot.SetActive(false);
}