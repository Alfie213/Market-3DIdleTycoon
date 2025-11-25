using UnityEngine;
using UnityEngine.UI;

public class WorldProgressBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject visuals;
    
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        Hide();
    }

    private void LateUpdate()
    {
        if (visuals.activeSelf)
        {
            // Полоска поворачивается к камере, но только по оси Y (чтобы не заваливалась назад)
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward, _mainCamera.transform.rotation * Vector3.up);
        }
    }

    public void SetProgress(float value)
    {
        if (!visuals.activeSelf) visuals.SetActive(true);
        slider.value = value;
    }

    public void Hide()
    {
        visuals.SetActive(false);
    }
}