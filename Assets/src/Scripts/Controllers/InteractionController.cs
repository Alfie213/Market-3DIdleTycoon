using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private LayerMask buildingLayer;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Блокировка нажатия, если курсор над UI (чтобы не кликать сквозь окно улучшений)
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Out");
                return;
            }

            HandleInput();
        }
    }

    private void HandleInput()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, buildingLayer))
        {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.TryGetComponent(out BuildingController building))
            {
                building.Interact();
            }
        }
        else
        {
            Debug.Log("No");
        }
    }
}