using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingObjectBase : MonoBehaviour
{
    [SerializeField] protected BuildingData buildingData;
    [SerializeField] private Transform queueStartPoint;
    [SerializeField] private Transform interactionPoint; 
    [SerializeField] private GameObject visualModel;
    [SerializeField] private GameObject constructionSiteVisuals;
    
    private bool _isBuilt = false;
    private bool _isProcessing = false;
    private readonly Queue<Customer> _customerQueue = new Queue<Customer>();

    public BuildingData Data => buildingData;
    public bool IsBuilt => _isBuilt;
    public Transform InteractionPoint => interactionPoint;

    private void Start()
    {
        UpdateVisuals();
    }

    public void Construct()
    {
        if (_isBuilt) return;
        
        _isBuilt = true;
        UpdateVisuals();
        GameEvents.InvokeBuildingConstructed(this);
    }

    private void UpdateVisuals()
    {
        if (visualModel) visualModel.SetActive(_isBuilt);
        if (constructionSiteVisuals) constructionSiteVisuals.SetActive(!_isBuilt);
    }

    public bool CanAcceptCustomer()
    {
        return _isBuilt && _customerQueue.Count < buildingData.MaxQueueCapacity;
    }

    public void EnqueueCustomer(Customer customer)
    {
        _customerQueue.Enqueue(customer);
        UpdateQueuePositions();
        
        if (!_isProcessing)
        {
            StartCoroutine(ProcessQueueRoutine());
        }
    }

    private IEnumerator ProcessQueueRoutine()
    {
        _isProcessing = true;

        while (_customerQueue.Count > 0)
        {
            Customer currentCustomer = _customerQueue.Peek();
            
            yield return new WaitForSeconds(buildingData.ProcessingTime);

            ProcessCustomerReward();
            currentCustomer.CompleteCurrentTask();
            _customerQueue.Dequeue();
            
            UpdateQueuePositions();
        }

        _isProcessing = false;
    }

    protected virtual void ProcessCustomerReward()
    {
        // Override in child classes if needed, or generic logic here
    }

    private void UpdateQueuePositions()
    {
        int index = 0;
        foreach (var customer in _customerQueue)
        {
            Vector3 targetPos = queueStartPoint.position - (queueStartPoint.forward * index * 1.5f);
            customer.MoveToPosition(targetPos);
            index++;
        }
    }
}