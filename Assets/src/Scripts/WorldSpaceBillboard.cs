using UnityEngine;

public class WorldSpaceBillboard : MonoBehaviour
{
    private Camera _mainCamera;

    protected virtual void Awake()
    {
        _mainCamera = Camera.main;
    }

    protected virtual void LateUpdate()
    {
        if (_mainCamera != null)
        {
            // Самый простой и эффективный способ для UI - копировать поворот камеры.
            // UI будет всегда параллелен плоскости экрана.
            transform.rotation = _mainCamera.transform.rotation;
        }
    }
}