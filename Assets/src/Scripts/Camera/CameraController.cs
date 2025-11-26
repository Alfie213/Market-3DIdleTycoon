using UnityEngine;
using UnityEngine.EventSystems; // Обязательно добавляем этот namespace

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Movement Limits")]
    [SerializeField] private Vector2 minPositionLimit;
    [SerializeField] private Vector2 maxPositionLimit;

    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float zoomSensitivity = 2f; 
    [SerializeField] private float touchZoomSensitivity = 0.05f; 
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float buttonZoomStep = 2f; 

    [Header("Movement Settings")]
    [SerializeField] private float groundHeight = 0f;
    [SerializeField] private float friction = 5f;

    private Camera _camera;
    private Plane _groundPlane;
    private Vector3 _dragOrigin;
    private Vector3 _velocity;
    private bool _isDragging;
    
    private float _targetZoom; 

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _groundPlane = new Plane(Vector3.up, new Vector3(0, groundHeight, 0));
        
        if (_camera.orthographic)
            _targetZoom = _camera.orthographicSize;
        else
            _targetZoom = _camera.fieldOfView;
    }

    private void LateUpdate()
    {
        HandleZoomInput();
        ApplyZoom();
        
        if (Input.touchCount >= 2)
        {
            _isDragging = false; 
            return;
        }

        HandleMovement();
    }

    // --- ZOOM LOGIC (Без изменений) ---
    private void HandleZoomInput()
    {
        // ... (код зума остался прежним) ...
        float scrollDelta = 0f;
        if (Input.mouseScrollDelta.y != 0) scrollDelta = -Input.mouseScrollDelta.y * zoomSensitivity;

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            scrollDelta = deltaMagnitudeDiff * touchZoomSensitivity;
        }

        if (Mathf.Abs(scrollDelta) > 0.01f) ModifyTargetZoom(scrollDelta);
    }

    private void ApplyZoom()
    {
        if (_camera.orthographic)
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetZoom, Time.deltaTime * zoomLerpSpeed);
        else
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _targetZoom, Time.deltaTime * zoomLerpSpeed);
    }

    // --- MOVEMENT LOGIC (ИСПРАВЛЕНО) ---

    private void HandleMovement()
    {
        // 1. НАЖАТИЕ
        if (Input.GetMouseButtonDown(0))
        {
            // !!! ГЛАВНОЕ ИСПРАВЛЕНИЕ !!!
            // Если мы нажали на UI элемент - выходим, не начинаем драг
            if (IsPointerOverUI()) return;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_groundPlane.Raycast(ray, out float entry))
            {
                _dragOrigin = ray.GetPoint(entry);
                _isDragging = true;
                _velocity = Vector3.zero;
            }
        }

        // 2. УДЕРЖАНИЕ
        if (Input.GetMouseButton(0) && _isDragging)
        {
            // Здесь проверку UI делать НЕ нужно.
            // Если игрок начал тянуть землю и случайно навел палец на кнопку UI в процессе,
            // камера не должна застревать.
            
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_groundPlane.Raycast(ray, out float entry))
            {
                Vector3 currentHit = ray.GetPoint(entry);
                Vector3 difference = _dragOrigin - currentHit;

                if (difference != Vector3.zero)
                {
                    transform.position += difference;
                    _velocity = difference / Time.deltaTime;
                }
            }
        }

        // 3. ОТПУСКАНИЕ
        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }

        // 4. ИНЕРЦИЯ
        if (!_isDragging)
        {
            if (_velocity.sqrMagnitude > 0.001f)
            {
                transform.position += _velocity * Time.deltaTime;
                _velocity = Vector3.Lerp(_velocity, Vector3.zero, friction * Time.deltaTime);
            }
        }
        
        ClampPosition();
    }

    // Вспомогательный метод для проверки UI (Работает и на ПК, и на Мобилке)
    private bool IsPointerOverUI()
    {
        // 1. Проверка для мыши или тача, который Unity эмулирует как мышь
        if (EventSystem.current.IsPointerOverGameObject()) return true;

        // 2. Проверка конкретно для тачей на мобилке
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return true;
            }
        }

        return false;
    }

    private void ClampPosition()
    {
        float x = Mathf.Clamp(transform.position.x, minPositionLimit.x, maxPositionLimit.x);
        float z = Mathf.Clamp(transform.position.z, minPositionLimit.y, maxPositionLimit.y);

        if (transform.position.x != x || transform.position.z != z)
        {
            transform.position = new Vector3(x, transform.position.y, z);
            _velocity = Vector3.zero;
        }
    }
    
    // ... ModifyTargetZoom, ZoomIn, ZoomOut, OnDrawGizmos (без изменений) ...
    private void ModifyTargetZoom(float amount)
    {
        _targetZoom += amount;
        _targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
    }
    public void ZoomIn() => ModifyTargetZoom(-buttonZoomStep);
    public void ZoomOut() => ModifyTargetZoom(buttonZoomStep);

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minPositionLimit.x + maxPositionLimit.x) / 2, 0, (minPositionLimit.y + maxPositionLimit.y) / 2);
        Vector3 size = new Vector3(maxPositionLimit.x - minPositionLimit.x, 1, maxPositionLimit.y - minPositionLimit.y);
        Gizmos.DrawWireCube(center, size);
    }
}