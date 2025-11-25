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
        // Поворот к камере (Billboard эффект)
        if (visuals.activeSelf)
        {
            transform.rotation = _mainCamera.transform.rotation;
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