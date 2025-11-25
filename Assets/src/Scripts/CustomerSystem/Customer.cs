using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Customer : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    
    private static readonly int IsMovingKey = Animator.StringToHash("IsMoving");

    private Queue<BuildingController> _shoppingList;
    private BuildingController _currentTarget;
    private Vector3 _exitPosition;
    private bool _isWaitingInQueue;

    private const float InteractionRadius = 2.0f; 

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    // ... (Initialize и GoToNextTarget без изменений) ...
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
                _agent.stoppingDistance = InteractionRadius; 
            }
        }
        else
        {
            _currentTarget = null;
            _agent.SetDestination(_exitPosition);
            _agent.stoppingDistance = 0f; 
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
        bool isMoving = _agent.velocity.sqrMagnitude > 0.1f;
        _animator.SetBool(IsMovingKey, isMoving);
    }

    private void HandleNavigation()
    {
        if (_currentTarget == null || _isWaitingInQueue) return;

        if (!_agent.pathPending)
        {
            float dist = _agent.remainingDistance;
            if (float.IsInfinity(dist)) dist = Vector3.Distance(transform.position, _currentTarget.InteractionPoint.position);

            if (dist <= InteractionRadius)
            {
                TryJoinQueue();
            }
        }
    }

    private void TryJoinQueue()
    {
        if (_currentTarget.CanAcceptCustomer())
        {
            _isWaitingInQueue = true;
            _agent.ResetPath(); 
            _currentTarget.EnqueueCustomer(this);
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        _agent.stoppingDistance = 0.1f; 
        _agent.SetDestination(position);
    }

    // НОВЫЙ МЕТОД: Проверяем, дошел ли агент до назначенной точки
    public bool IsAtTargetPosition()
    {
        if (_agent.pathPending) return false;
        // Считаем, что дошли, если дистанция меньше чуть большего порога, чем stoppingDistance
        return _agent.remainingDistance <= _agent.stoppingDistance + 0.1f;
    }

    public void CompleteCurrentTask()
    {
        GoToNextTarget();
    }
}