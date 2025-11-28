using UnityEngine;

/// <summary>
/// Automatically adjusts the attached RectTransform to fit within the device's screen safe area.
/// Handles mobile notches, rounded corners, and orientation changes.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform _rectTransform;
    private ScreenOrientation _lastOrientation;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void Start()
    {
        _lastOrientation = Screen.orientation;
    }

    private void Update()
    {
        // Detect orientation change to re-apply bounds
        if (_lastOrientation != Screen.orientation)
        {
            ApplySafeArea();
            _lastOrientation = Screen.orientation;
        }
    }

    /// <summary>
    /// Calculates the safe area in normalized coordinates and updates the anchors.
    /// </summary>
    private void ApplySafeArea()
    {
        var safeArea = Screen.safeArea;
        var anchorMin = safeArea.position;
        var anchorMax = safeArea.position + safeArea.size;

        // Convert pixel values to normalized anchor values (0 to 1)
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}