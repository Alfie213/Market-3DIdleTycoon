using System.Collections;
using UnityEngine;

// Класс переименован, так как подходит и для UI, и для 3D объектов
public class PulseAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float pulseScale = 1.1f; // Для 3D лучше ставить поменьше (например 1.1)
    [SerializeField] private float duration = 0.15f;

    private Vector3 _originalScale;
    private Coroutine _animationCoroutine;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void Play()
    {
        if (!gameObject.activeInHierarchy) return;

        if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        float timer = 0f;
        Vector3 targetScale = _originalScale * pulseScale;

        // Zoom In
        while (timer < duration / 2)
        {
            timer += Time.deltaTime;
            float progress = timer / (duration / 2);
            transform.localScale = Vector3.Lerp(_originalScale, targetScale, progress);
            yield return null;
        }

        // Zoom Out
        timer = 0f;
        while (timer < duration / 2)
        {
            timer += Time.deltaTime;
            float progress = timer / (duration / 2);
            transform.localScale = Vector3.Lerp(targetScale, _originalScale, progress);
            yield return null;
        }

        transform.localScale = _originalScale;
        _animationCoroutine = null;
    }

    private void OnDisable()
    {
        transform.localScale = _originalScale;
    }
}