using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerPoint : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject workerModel;
    [SerializeField] private WorldProgressBar progressBar;
    
    [Header("Configuration")]
    [SerializeField] private Transform queueOrigin; 

    public event Action OnStateChanged;

    private bool _isUnlocked = false;
    private bool _isBusy = false;
    private float _processingTime;
    private int _profit;
    
    private readonly Queue<Customer> _localQueue = new Queue<Customer>();

    public bool IsUnlocked => _isUnlocked;
    public bool IsBusy => _isBusy;
    public int QueueCount => _localQueue.Count;

    private void Awake()
    {
        if (progressBar) progressBar.Hide();
    }

    public void Initialize(float processTime, int profit)
    {
        _processingTime = processTime;
        _profit = profit;
    }

    public void SetUnlocked(bool state)
    {
        _isUnlocked = state;
        if (workerModel) workerModel.SetActive(state);
        
        if (!state)
        {
            progressBar.Hide();
        }
        OnStateChanged?.Invoke();
    }

    public void EnqueueCustomer(Customer customer)
    {
        _localQueue.Enqueue(customer);
        UpdateQueuePositions();
        TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (_isBusy || _localQueue.Count == 0) return;
        
        StartCoroutine(ProcessRoutine());
    }

    private IEnumerator ProcessRoutine()
    {
        _isBusy = true;
        OnStateChanged?.Invoke();

        Customer currentCustomer = _localQueue.Peek();
        
        currentCustomer.MoveToPosition(queueOrigin.position);

        float arrivalTimeout = 10f; 
        while (!currentCustomer.IsAtTargetPosition() && arrivalTimeout > 0)
        {
            arrivalTimeout -= Time.deltaTime;
            yield return null;
        }

        float timer = 0f;
        while (timer < _processingTime)
        {
            timer += Time.deltaTime;
            if (progressBar) progressBar.SetProgress(timer / _processingTime);
            yield return null;
        }

        // --- ИСПРАВЛЕНИЕ ЗДЕСЬ ---
        // Проверяем, приносит ли это действие деньги
        if (_profit > 0)
        {
            CurrencyController.Instance.AddCurrency(_profit);
            
            // Вызываем событие ТОЛЬКО если была прибыль (продажа)
            GameEvents.InvokeCustomerServed();
        }
        // -------------------------

        currentCustomer.CompleteCurrentTask();
        _localQueue.Dequeue();

        if (progressBar) progressBar.Hide();
        _isBusy = false;
        OnStateChanged?.Invoke();

        UpdateQueuePositions();
        TryProcessNext();
    }

    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (var customer in _localQueue)
        {
            Vector3 targetPos = queueOrigin.position - (queueOrigin.forward * index * 1.5f);
            customer.MoveToPosition(targetPos);
            index++;
        }
    }
}