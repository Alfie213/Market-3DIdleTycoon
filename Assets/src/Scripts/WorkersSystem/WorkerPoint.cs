using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single service point (e.g., one cashier or one stall worker).
/// Handles its own customer queue and processing logic.
/// </summary>
public class WorkerPoint : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject workerModel;
    [SerializeField] private WorldProgressBar progressBar;
    
    [Header("Configuration")]
    [Tooltip("The position where the customer stands while being served. Z-Axis should point towards the queue line.")]
    [SerializeField] private Transform queueOrigin;
    [SerializeField] private float queueSpacing = 1.3f;

    /// <summary>
    /// Event fired when worker becomes busy/free or unlocked/locked.
    /// </summary>
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
        if (!state) progressBar.Hide();
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

    /// <summary>
    /// Main logic loop: Wait for customer -> Process -> Reward -> Next.
    /// </summary>
    private IEnumerator ProcessRoutine()
    {
        _isBusy = true;
        OnStateChanged?.Invoke();

        Customer currentCustomer = _localQueue.Peek();
        currentCustomer.MoveToPosition(queueOrigin.position);

        // 1. Wait for customer to physically arrive at the desk
        float arrivalTimeout = 10f; 
        while (!currentCustomer.IsAtTargetPosition() && arrivalTimeout > 0)
        {
            arrivalTimeout -= Time.deltaTime;
            yield return null;
        }

        // 2. Processing (Progress Bar)
        float timer = 0f;
        while (timer < _processingTime)
        {
            timer += Time.deltaTime;
            if (progressBar) progressBar.SetProgress(timer / _processingTime);
            yield return null;
        }

        // 3. Give Reward
        if (_profit > 0)
        {
            CurrencyController.Instance.AddCurrency(_profit);
            GameEvents.InvokeSaleCompleted();
        }

        // 4. Cleanup and Next
        currentCustomer.CompleteCurrentTask();
        _localQueue.Dequeue();

        if (progressBar) progressBar.Hide();
        _isBusy = false;
        OnStateChanged?.Invoke();

        UpdateQueuePositions();
        TryProcessNext();
    }

    /// <summary>
    /// Re-calculates positions for all customers in the queue.
    /// </summary>
    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (var customer in _localQueue)
        {
            // Skip the first customer if they are currently being processed
            if (_isBusy && index == 0)
            {
                index++;
                continue;
            }

            Vector3 targetPos = queueOrigin.position - (queueOrigin.forward * index * queueSpacing);
            customer.MoveToPosition(targetPos);
            index++;
        }
    }
}