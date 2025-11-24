using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;
    
    [Header("Visuals")]
    [SerializeField] private GameObject visualModel;
    [SerializeField] private GameObject constructionSiteVisuals;
    
    [Header("Points")]
    [SerializeField] private Transform interactionPoint; // Куда идут NPC
    [SerializeField] private Transform queueStartPoint;  // Где начинается очередь

    private bool _isBuilt = false;
    private readonly Queue<Customer> _customerQueue = new Queue<Customer>();
    
    // Runtime Stats (могут улучшаться)
    private int _currentMaxWorkers;
    private float _currentProcessingTime;
    private int _activeWorkersCount = 0;

    public BuildingData Data => buildingData;
    public bool IsBuilt => _isBuilt;
    public Transform InteractionPoint => interactionPoint;
    
    // Свойства для UI улучшений
    public int CurrentMaxWorkers => _currentMaxWorkers;
    public float CurrentProcessingTime => _currentProcessingTime;

    private void Awake()
    {
        // Инициализация статов из Data
        _currentMaxWorkers = buildingData.BaseMaxWorkers;
        _currentProcessingTime = buildingData.BaseProcessingTime;
        
        UpdateVisuals();
    }

    // Метод взаимодействия (вызывается из InteractionController)
    public void Interact()
    {
        if (!_isBuilt)
        {
            TryConstruct();
        }
        else
        {
            // Здание уже построено -> Открываем окно улучшений
            GameEvents.InvokeUpgradeWindowRequested(this);
        }
    }

    private void TryConstruct()
    {
        if (CurrencyController.Instance.TrySpendCurrency(buildingData.BuildCost))
        {
            _isBuilt = true;
            UpdateVisuals();
            GameEvents.InvokeBuildingConstructed(this);
        }
    }

    public void UpgradeSpeed()
    {
        // Пример логики: уменьшаем время на 10%, минимум 0.5 сек
        _currentProcessingTime = Mathf.Max(0.5f, _currentProcessingTime * 0.9f);
    }

    public void UpgradeWorkers()
    {
        _currentMaxWorkers++;
        // Если очередь ждала свободного работника, запускаем обработку
        TryProcessNextCustomer();
    }

    private void UpdateVisuals()
    {
        if (visualModel) visualModel.SetActive(_isBuilt);
        if (constructionSiteVisuals) constructionSiteVisuals.SetActive(!_isBuilt);
    }

    // --- Логика Очереди и Клиентов ---

    public bool CanAcceptCustomer()
    {
        return _isBuilt && _customerQueue.Count < buildingData.MaxQueueCapacity;
    }

    public void EnqueueCustomer(Customer customer)
    {
        _customerQueue.Enqueue(customer);
        UpdateQueuePositions();
        TryProcessNextCustomer();
    }

    private void TryProcessNextCustomer()
    {
        // Если есть клиенты И есть свободные работники
        if (_customerQueue.Count > 0 && _activeWorkersCount < _currentMaxWorkers)
        {
            StartCoroutine(ProcessCustomerRoutine());
        }
    }

    private IEnumerator ProcessCustomerRoutine()
    {
        _activeWorkersCount++;
        
        // Берем клиента, но пока не удаляем из очереди (он стоит у кассы)
        Customer currentCustomer = _customerQueue.Peek();
        
        yield return new WaitForSeconds(_currentProcessingTime);

        // Обработка завершена
        CompleteTransaction();
        
        // Отпускаем клиента
        currentCustomer.CompleteCurrentTask();
        _customerQueue.Dequeue();
        
        _activeWorkersCount--;
        
        UpdateQueuePositions();
        
        // Пробуем взять следующего, если есть свободные слоты (рекурсия через событие завершения)
        TryProcessNextCustomer(); 
    }

    private void CompleteTransaction()
    {
        if (buildingData.ProfitPerCustomer > 0)
        {
            CurrencyController.Instance.AddCurrency(buildingData.ProfitPerCustomer);
        }
    }

    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (var customer in _customerQueue)
        {
            // Формируем очередь назад от точки старта
            // Z - backward, чтобы очередь выстраивалась "назад"
            Vector3 targetPos = queueStartPoint.position - (queueStartPoint.forward * index * 1.5f);
            customer.MoveToPosition(targetPos);
            index++;
        }
    }
}