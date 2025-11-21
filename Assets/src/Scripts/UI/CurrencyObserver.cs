using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyObserver : MonoBehaviour
{
    [SerializeField] private CurrencyController currencyController;
    [SerializeField] private TextMeshProUGUI currencyCountTMP;

    private void OnEnable()
    {
        currencyController.OnCurrencyCountChange += EditCurrencyCount;
    }

    private void OnDisable()
    {
        currencyController.OnCurrencyCountChange -= EditCurrencyCount;
    }

    private void EditCurrencyCount(int currencyCount)
    {
        currencyCountTMP.text = currencyCount.ToString();
    }
}
