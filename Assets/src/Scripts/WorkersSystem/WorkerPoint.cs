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
    [Tooltip("Точка, где стоит ПЕРВЫЙ клиент в этой очереди")]
    [SerializeField] private Transform queueOrigin; 

    // Событие, чтобы уведомить здание об изменении статуса (для UI)
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
        // Скрываем бар при старте
        if (progressBar) progressBar.Hide();
    }

    // Инициализация данных (вызывается из BuildingController)
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
            // Тут можно добавить логику разгона очереди, если точку закрыли,
            // но для прототипа считаем, что закрыть купленное нельзя.
        }
        OnStateChanged?.Invoke();
    }

    public void EnqueueCustomer(Customer customer)
    {
        _localQueue.Enqueue(customer);
        UpdateQueuePositions();
        
        // Если работник свободен, начинаем обслуживание
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
        
        // Логика прогресс-бара
        float timer = 0f;
        while (timer < _processingTime)
        {
            timer += Time.deltaTime;
            if (progressBar) progressBar.SetProgress(timer / _processingTime);
            yield return null;
        }

        // Награда
        if (_profit > 0)
        {
            CurrencyController.Instance.AddCurrency(_profit);
        }

        // Отпускаем клиента
        currentCustomer.CompleteCurrentTask();
        _localQueue.Dequeue();

        // Сброс состояния
        if (progressBar) progressBar.Hide();
        _isBusy = false;
        OnStateChanged?.Invoke();

        // Двигаем очередь и берем следующего
        UpdateQueuePositions();
        TryProcessNext();
    }

    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (var customer in _localQueue)
        {
            // Строим очередь назад от queueOrigin этого конкретного работника
            Vector3 targetPos = queueOrigin.position - (queueOrigin.forward * index * 1.5f);
            customer.MoveToPosition(targetPos);
            index++;
        }
    }
}