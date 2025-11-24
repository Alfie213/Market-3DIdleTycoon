using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Movement Limits")]
    [SerializeField] private Vector2 minPositionLimit;
    [SerializeField] private Vector2 maxPositionLimit;

    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float zoomSensitivity = 2f; // Чувствительность колесика
    [SerializeField] private float touchZoomSensitivity = 0.05f; // Чувствительность щипка
    [SerializeField] private float zoomLerpSpeed = 10f; // Скорость сглаживания зума

    [Header("Movement Settings")]
    [SerializeField] private float groundHeight = 0f;
    [SerializeField] private float friction = 5f;

    private Camera _camera;
    private Plane _groundPlane;
    private Vector3 _dragOrigin;
    private Vector3 _velocity;
    private bool _isDragging;
    
    // Целевое значение зума для плавности
    private float _targetZoom; 

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _groundPlane = new Plane(Vector3.up, new Vector3(0, groundHeight, 0));
        
        // Инициализируем текущим зумом
        if (_camera.orthographic)
            _targetZoom = _camera.orthographicSize;
        else
            _targetZoom = _camera.fieldOfView;
    }

    private void LateUpdate()
    {
        HandleZoom();
        
        // Если мы зумим пальцами, перемещение отключаем, чтобы не сбивать камеру
        if (Input.touchCount >= 2)
        {
            _isDragging = false; 
            return;
        }

        HandleMovement();
    }

    private void HandleZoom()
    {
        float scrollDelta = 0f;

        // 1. Логика для мыши
        if (Input.mouseScrollDelta.y != 0)
        {
            scrollDelta = -Input.mouseScrollDelta.y * zoomSensitivity;
        }

        // 2. Логика для тача (Pinch to Zoom)
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

        // Применяем изменение к таргету
        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            _targetZoom += scrollDelta;
            _targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
        }

        // Плавное применение зума
        if (_camera.orthographic)
        {
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetZoom, Time.deltaTime * zoomLerpSpeed);
        }
        else
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _targetZoom, Time.deltaTime * zoomLerpSpeed);
        }
    }

    private void HandleMovement()
    {
        // Нажатие
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_groundPlane.Raycast(ray, out float entry))
            {
                _dragOrigin = ray.GetPoint(entry);
                _isDragging = true;
                _velocity = Vector3.zero;
            }
        }

        // Удержание
        if (Input.GetMouseButton(0) && _isDragging)
        {
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

        // Отпускание
        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }

        // Инерция (работает, когда не держим палец)
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
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minPositionLimit.x + maxPositionLimit.x) / 2, 0, (minPositionLimit.y + maxPositionLimit.y) / 2);
        Vector3 size = new Vector3(maxPositionLimit.x - minPositionLimit.x, 1, maxPositionLimit.y - minPositionLimit.y);
        Gizmos.DrawWireCube(center, size);
    }
}