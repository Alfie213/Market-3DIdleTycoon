using UnityEngine;

/// <summary>
/// Forces object to always face the camera. Used for World UI.
/// </summary>
public class WorldSpaceBillboard : MonoBehaviour
{
    private Camera _mainCamera;
    protected virtual void Awake() => _mainCamera = Camera.main;
    
    protected virtual void LateUpdate()
    {
        if (_mainCamera != null) transform.rotation = _mainCamera.transform.rotation;
    }
}