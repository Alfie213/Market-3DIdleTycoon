using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private LayerMask buildingLayer;
    
    [Tooltip("Если курсор сместился больше чем на это кол-во пикселей, клик отменяется (считаем это драгом камеры)")]
    [SerializeField] private float dragThreshold = 40f; 

    private Camera _mainCamera;
    
    // Храним здание, на которое нажали в начале
    private BuildingController _pressedBuilding;
    // Храним позицию нажатия, чтобы считать дистанцию свайпа
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
        // 1. НАЖАТИЕ (Mouse Down)
        // Здесь мы ТОЛЬКО запоминаем, на что нажали. Никаких действий не совершаем.
        if (Input.GetMouseButtonDown(0))
        {
            // Если нажали на UI (кнопку постройки и т.д.), игнорируем 3D мир
            if (IsPointerOverUI()) 
            {
                _pressedBuilding = null;
                return;
            }

            _pressedBuilding = GetBuildingUnderCursor();
            _pointerDownPosition = Input.mousePosition;
        }

        // 2. ОТПУСКАНИЕ (Mouse Up)
        // Все проверки и действия происходят только здесь.
        if (Input.GetMouseButtonUp(0))
        {
            // Если в начале мы нажали в пустоту или на UI - выходим
            if (_pressedBuilding == null) return;

            // А. Проверка на свайп камеры
            // Если курсор уехал далеко от места нажатия - это было перемещение камеры, а не клик.
            float distance = Vector3.Distance(_pointerDownPosition, Input.mousePosition);
            if (distance > dragThreshold)
            {
                _pressedBuilding = null;
                return;
            }

            // Б. Проверка, что мы отпустили курсор НАД ТЕМ ЖЕ зданием
            BuildingController releasedBuilding = GetBuildingUnderCursor();

            if (releasedBuilding != null && releasedBuilding == _pressedBuilding)
            {
                // Дополнительная проверка, не перекрыт ли курсор UI сейчас (на всякий случай)
                if (!IsPointerOverUI())
                {
                    // ВЫЗЫВАЕМ ДЕЙСТВИЕ (Стройка или Окно улучшений)
                    _pressedBuilding.Interact();
                }
            }

            // Сбрасываем ссылку
            _pressedBuilding = null;
        }
    }

    // Универсальная проверка UI (ПК + Мобайл)
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

    private BuildingController GetBuildingUnderCursor()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, buildingLayer))
        {
            if (hit.collider.TryGetComponent(out BuildingController building))
            {
                return building;
            }
        }
        return null;
    }
}