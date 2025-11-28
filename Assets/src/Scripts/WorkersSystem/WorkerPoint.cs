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
    [Tooltip("Точка, где стоит ОБСЛУЖИВАЕМЫЙ клиент. Синяя стрелка (Z) должна смотреть в лицо кассиру!")]
    [SerializeField] private Transform queueOrigin;
    [SerializeField] private float queueSpacing = 1.3f; // Расстояние между людьми

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

        // Берем первого, но НЕ удаляем из очереди (он все еще занимает место)
        Customer currentCustomer = _localQueue.Peek();
        
        // 1. Клиент идет к кассе
        currentCustomer.MoveToPosition(queueOrigin.position);

        // Ждем пока дойдет
        float arrivalTimeout = 10f; 
        while (!currentCustomer.IsAtTargetPosition() && arrivalTimeout > 0)
        {
            arrivalTimeout -= Time.deltaTime;
            yield return null;
        }

        // 2. Обработка (прогресс бар)
        float timer = 0f;
        while (timer < _processingTime)
        {
            timer += Time.deltaTime;
            if (progressBar) progressBar.SetProgress(timer / _processingTime);
            yield return null;
        }

        // 3. Награда
        if (_profit > 0)
        {
            CurrencyController.Instance.AddCurrency(_profit);
            GameEvents.InvokeSaleCompleted();
        }

        // 4. Завершение
        currentCustomer.CompleteCurrentTask();
        
        // Удаляем из очереди только СЕЙЧАС
        _localQueue.Dequeue();

        if (progressBar) progressBar.Hide();
        _isBusy = false;
        OnStateChanged?.Invoke();

        // Сдвигаем остальных
        UpdateQueuePositions();
        
        // Берем следующего
        TryProcessNext();
    }

    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (var customer in _localQueue)
        {
            // Если мы сейчас заняты обработкой, то нулевой клиент (тот, кого обслуживают)
            // уже управляется корутиной ProcessRoutine. Его трогать не надо, чтобы не дергать анимацию.
            // Двигаем только хвост (index > 0).
            // Если мы НЕ заняты (клиент только идет к пустой кассе), то двигаем и его.
            
            if (_isBusy && index == 0)
            {
                index++;
                continue;
            }

            // Формула: Встаем в Origin и отступаем назад (прочь от forward)
            Vector3 targetPos = queueOrigin.position - (queueOrigin.forward * index * queueSpacing);
            customer.MoveToPosition(targetPos);
            index++;
        }
    }

    // --- ОТЛАДКА В РЕДАКТОРЕ ---
    private void OnDrawGizmos()
    {
        if (queueOrigin == null) return;

        // 1. Точка обслуживания (ЗЕЛЕНАЯ) - Тот, кто платит сейчас
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(queueOrigin.position, 0.4f);

        // 2. Хвост очереди (КРАСНЫЙ) - Те, кто ждут
        for (int i = 1; i < 6; i++)
        {
            Vector3 pos = queueOrigin.position - (queueOrigin.forward * i * queueSpacing);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pos, 0.25f);
            
            // Рисуем линии соединения (ЖЕЛТЫЕ)
            Gizmos.color = Color.yellow;
            if (i > 1)
            {
                Vector3 prevPos = queueOrigin.position - (queueOrigin.forward * (i - 1) * queueSpacing);
                Gizmos.DrawLine(prevPos, pos);
            }
            else
            {
                // Линия от зеленой точки к первой красной
                Gizmos.DrawLine(queueOrigin.position, pos);
            }
        }
    }
}