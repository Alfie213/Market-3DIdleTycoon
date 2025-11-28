using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// RTS-style camera with drag-to-move and pinch/scroll-to-zoom.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Limits")]
    [SerializeField] private Vector2 minPositionLimit;
    [SerializeField] private Vector2 maxPositionLimit;

    [Header("Zoom")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float zoomSensitivity = 2f; 
    [SerializeField] private float touchZoomSensitivity = 0.05f; 
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float buttonZoomStep = 2f; 

    [Header("Movement")]
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
        _targetZoom = _camera.orthographic ? _camera.orthographicSize : _camera.fieldOfView;
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

    private void HandleZoomInput()
    {
        float scrollDelta = 0f;
        if (Input.mouseScrollDelta.y != 0) scrollDelta = -Input.mouseScrollDelta.y * zoomSensitivity;

        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);
            float prevDelta = ((t0.position - t0.deltaPosition) - (t1.position - t1.deltaPosition)).magnitude;
            float curDelta = (t0.position - t1.position).magnitude;
            scrollDelta = (prevDelta - curDelta) * touchZoomSensitivity;
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

    private void HandleMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI()) return;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (_groundPlane.Raycast(ray, out float entry))
            {
                _dragOrigin = ray.GetPoint(entry);
                _isDragging = true;
                _velocity = Vector3.zero;
            }
        }

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

        if (Input.GetMouseButtonUp(0)) _isDragging = false;

        if (!_isDragging && _velocity.sqrMagnitude > 0.001f)
        {
            transform.position += _velocity * Time.deltaTime;
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, friction * Time.deltaTime);
        }
        
        ClampPosition();
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return true;
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(t.fingerId)) return true;
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
    
    public void ZoomIn() => ModifyTargetZoom(-buttonZoomStep);
    public void ZoomOut() => ModifyTargetZoom(buttonZoomStep);
    private void ModifyTargetZoom(float amount) => _targetZoom = Mathf.Clamp(_targetZoom + amount, minZoom, maxZoom);
}