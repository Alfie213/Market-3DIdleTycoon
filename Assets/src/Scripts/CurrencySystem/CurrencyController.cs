using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    public event Action<int> OnCurrencyCountChange;
    
    private int _currencyCount;

    public bool TryDecreaseCurrency(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Amount < 0");
            return false;
        }

        if (_currencyCount - amount < 0) return false;
        
        _currencyCount -= amount;
        OnCurrencyCountChange?.Invoke(_currencyCount);
        return true;
    }
}
