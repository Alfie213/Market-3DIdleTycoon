using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector2 minLimit;
    [SerializeField] private Vector2 maxLimit;
    [SerializeField] private float groundHeight = 0f;
    [SerializeField] private float friction = 5f; 

    private Camera _camera;
    private Plane _groundPlane;
    private Vector3 _dragOrigin;
    private Vector3 _velocity;
    private bool _isDragging;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _groundPlane = new Plane(Vector3.up, new Vector3(0, groundHeight, 0));
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }
        else if (Input.GetMouseButton(0) && _isDragging)
        {
            ProcessDrag();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }
        else
        {
            ProcessInertia();
        }

        ClampPosition();
    }

    private void StartDrag()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (_groundPlane.Raycast(ray, out float entry))
        {
            _dragOrigin = ray.GetPoint(entry);
            _isDragging = true;
            _velocity = Vector3.zero;
        }
    }

    private void ProcessDrag()
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

    private void StopDrag()
    {
        _isDragging = false;
    }

    private void ProcessInertia()
    {
        if (_velocity.sqrMagnitude > 0.001f)
        {
            transform.position += _velocity * Time.deltaTime;
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, friction * Time.deltaTime);
        }
    }

    private void ClampPosition()
    {
        float x = Mathf.Clamp(transform.position.x, minLimit.x, maxLimit.x);
        float z = Mathf.Clamp(transform.position.z, minLimit.y, maxLimit.y);
        
        if (transform.position.x != x || transform.position.z != z)
        {
            transform.position = new Vector3(x, transform.position.y, z);
            _velocity = Vector3.zero; 
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minLimit.x + maxLimit.x) / 2, 0, (minLimit.y + maxLimit.y) / 2);
        Vector3 size = new Vector3(maxLimit.x - minLimit.x, 1, maxLimit.y - minLimit.y);
        Gizmos.DrawWireCube(center, size);
    }
}