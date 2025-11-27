using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer; // Переименовали для ясности (было buildingLayer)
    [SerializeField] private float dragThreshold = 40f; 

    private Camera _mainCamera;
    
    // Теперь храним интерфейс, а не конкретный класс здания
    private IInteractable _pressedObject;
    private Vector3 _pointerDownPosition;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // 1. НАЖАТИЕ
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI()) 
            {
                _pressedObject = null;
                return;
            }

            _pressedObject = GetInteractableUnderCursor();
            _pointerDownPosition = Input.mousePosition;
        }

        // 2. ОТПУСКАНИЕ
        if (Input.GetMouseButtonUp(0))
        {
            if (_pressedObject == null) return;

            float distance = Vector3.Distance(_pointerDownPosition, Input.mousePosition);
            if (distance > dragThreshold)
            {
                _pressedObject = null;
                return;
            }

            IInteractable releasedObject = GetInteractableUnderCursor();

            // Сравниваем объекты. Если это один и тот же объект (и он не null) -> Interact
            // (Сравниваем как объекты Unity, так как интерфейсы напрямую сравнивать опасно, если объект уничтожен)
            if (releasedObject != null && releasedObject == _pressedObject)
            {
                if (!IsPointerOverUI())
                {
                    _pressedObject.Interact();
                }
            }

            _pressedObject = null;
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return true;
            }
        }
        return false;
    }

    // Универсальный метод поиска
    private IInteractable GetInteractableUnderCursor()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                return interactable;
            }
        }
        return null;
    }
}