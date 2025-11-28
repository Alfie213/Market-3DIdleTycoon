using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles raycasting logic for clicking on 3D objects (buildings, ATMs).
/// Supports drag threshold to distinguish clicks from camera movement.
/// </summary>
public class InteractionController : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float dragThreshold = 40f; 

    private Camera _mainCamera;
    private IInteractable _pressedObject;
    private Vector3 _pointerDownPosition;

    private void Awake() => _mainCamera = Camera.main;

    private void Update() => HandleInput();

    private void HandleInput()
    {
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
            if (releasedObject != null && releasedObject == _pressedObject)
            {
                if (!IsPointerOverUI()) _pressedObject.Interact();
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

    private IInteractable GetInteractableUnderCursor()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, interactableLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable)) return interactable;
        }
        return null;
    }
}