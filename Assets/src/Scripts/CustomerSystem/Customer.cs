using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    
    private Queue<BuildingObjectBase> _shoppingList;
    private BuildingObjectBase _currentTarget;
    private Vector3 _exitPosition;
    private bool _isWaitingInQueue;

    public void Initialize(List<BuildingObjectBase> route, Vector3 exitPos)
    {
        _shoppingList = new Queue<BuildingObjectBase>(route);
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
                agent.SetDestination(_currentTarget.InteractionPoint.position);
            }
        }
        else
        {
            _currentTarget = null;
            agent.SetDestination(_exitPosition);
            Destroy(gameObject, 5f); // Safety cleanup
        }
    }

    private void Update()
    {
        if (_currentTarget == null || _isWaitingInQueue) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
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
            // If queue full, wait or leave. For prototype, we just wait near interaction point.
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        agent.SetDestination(position);
    }

    public void CompleteCurrentTask()
    {
        GoToNextTarget();
    }
}