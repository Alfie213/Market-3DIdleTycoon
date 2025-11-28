using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NPC logic using NavMeshAgent and Animator. Handles movement between buildings and queueing.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Customer : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Animator _animator;
    
    private static readonly int IsMovingKey = Animator.StringToHash(GameConstants.AnimParamIsMoving);
    private const float InteractionRadius = 2.0f; 

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

    public void MoveToPosition(Vector3 position)
    {
        _agent.stoppingDistance = 0.1f; 
        _agent.SetDestination(position);
    }

    public bool IsAtTargetPosition()
    {
        if (_agent.pathPending) return false;
        return _agent.remainingDistance <= _agent.stoppingDistance + 0.1f;
    }

    public void CompleteCurrentTask() => GoToNextTarget();
}