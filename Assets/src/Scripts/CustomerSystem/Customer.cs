using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Customer : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Queue<BuildingController> _shoppingList;
    private BuildingController _currentTarget;
    private Vector3 _exitPosition;
    private bool _isWaitingInQueue;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
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
            _currentTarget.EnqueueCustomer(this);
        }
        else
        {
            // Простая логика: если очередь полна, ждем на месте (idle)
            // В будущем здесь можно сделать логику ухода или ожидания в стороне
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        // Используется, когда NPC уже в очереди, чтобы двигаться за людьми
        _agent.SetDestination(position);
    }

    public void CompleteCurrentTask()
    {
        GoToNextTarget();
    }
}