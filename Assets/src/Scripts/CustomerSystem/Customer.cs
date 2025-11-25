using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Customer : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    
    // Хеширование имени параметра для оптимизации (быстрее, чем сравнение строк каждый кадр)
    private static readonly int IsMovingKey = Animator.StringToHash("IsMoving");

    private Queue<BuildingController> _shoppingList;
    private BuildingController _currentTarget;
    private Vector3 _exitPosition;
    private bool _isWaitingInQueue;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    public void Initialize(BuildingController[] route, Vector3 exitPos)
    {
        _shoppingList = new Queue<BuildingController>(route);
        _exitPosition = exitPos;
        GoToNextTarget();
    }

    private void GoToNextTarget()
    {
        _isWaitingInQueue = false;

        if (_shoppingList.Count > 0)
        {
            _currentTarget = _shoppingList.Dequeue();
            if (_currentTarget != null)
            {
                _agent.SetDestination(_currentTarget.InteractionPoint.position);
            }
        }
        else
        {
            _currentTarget = null;
            _agent.SetDestination(_exitPosition);
            Destroy(gameObject, 5f); 
        }
    }

    private void Update()
    {
        HandleAnimations();
        HandleNavigation();
    }

    private void HandleAnimations()
    {
        // Проверяем квадрат магнитуды скорости (это быстрее, чем обычная magnitude)
        // 0.1f — небольшой порог, чтобы отсеять микро-движения агента
        bool isMoving = _agent.velocity.sqrMagnitude > 0.1f;
        
        // Передаем значение в аниматор
        _animator.SetBool(IsMovingKey, isMoving);
    }

    private void HandleNavigation()
    {
        if (_currentTarget == null || _isWaitingInQueue) return;

        // Проверка: дошли ли до точки взаимодействия
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            TryJoinQueue();
        }
    }

    private void TryJoinQueue()
    {
        if (_currentTarget.CanAcceptCustomer())
        {
            _isWaitingInQueue = true;
            // При остановке в очереди агент сам сбросит скорость в 0, 
            // и HandleAnimations автоматически включит Idle
            _agent.ResetPath(); 
            _currentTarget.EnqueueCustomer(this);
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        // Когда очередь двигается, агент получает новую точку -> скорость растет -> включается Walk
        _agent.SetDestination(position);
    }

    public void CompleteCurrentTask()
    {
        GoToNextTarget();
    }
}