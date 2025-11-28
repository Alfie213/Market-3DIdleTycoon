using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a single worker/cashier spot, its local queue, and processing routine.
/// </summary>
public class WorkerPoint : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject workerModel;
    [SerializeField] private WorldProgressBar progressBar;
    
    [Header("Queue Config")]
    [SerializeField] private Transform queueOrigin;
    [SerializeField] private float queueSpacing = 1.3f;

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

    private IEnumerator ProcessRoutine()
    {
        _isBusy = true;
        OnStateChanged?.Invoke();

        Customer currentCustomer = _localQueue.Peek();
        currentCustomer.MoveToPosition(queueOrigin.position);

        // Wait for customer to arrive
        float arrivalTimeout = 10f; 
        while (!currentCustomer.IsAtTargetPosition() && arrivalTimeout > 0)
        {
            arrivalTimeout -= Time.deltaTime;
            yield return null;
        }

        // Process (Progress Bar)
        float timer = 0f;
        while (timer < _processingTime)
        {
            timer += Time.deltaTime;
            if (progressBar) progressBar.SetProgress(timer / _processingTime);
            yield return null;
        }

        // Reward
        if (_profit > 0)
        {
            CurrencyController.Instance.AddCurrency(_profit);
            GameEvents.InvokeSaleCompleted();
        }

        // Cleanup
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
            // If busy, skip the first person (they are at the counter)
            if (_isBusy && index == 0)
            {
                index++;
                continue;
            }

            Vector3 targetPos = queueOrigin.position - (queueOrigin.forward * (index * queueSpacing));
            customer.MoveToPosition(targetPos);
            index++;
        }
    }

    private void OnDrawGizmos()
    {
        if (queueOrigin == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(queueOrigin.position, 0.4f);

        for (int i = 1; i < 6; i++)
        {
            Vector3 pos = queueOrigin.position - (queueOrigin.forward * i * queueSpacing);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pos, 0.25f);
            Gizmos.color = Color.yellow;
            if (i > 1)
            {
                Vector3 prevPos = queueOrigin.position - (queueOrigin.forward * (i - 1) * queueSpacing);
                Gizmos.DrawLine(prevPos, pos);
            }
            else Gizmos.DrawLine(queueOrigin.position, pos);
        }
    }
}