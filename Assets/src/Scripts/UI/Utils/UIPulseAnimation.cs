using System.Collections;
using UnityEngine;

public class UIPulseAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float pulseScale = 1.2f; // Насколько увеличивать (1.2 = на 20%)
    [SerializeField] private float duration = 0.2f;   // Как быстро (в секундах)

    private Vector3 _originalScale;
    private Coroutine _animationCoroutine;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void Play()
    {
        // Если объект выключен, корутина не запустится, поэтому включаем (на всякий случай)
        if (!gameObject.activeInHierarchy) return;

        if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        float timer = 0f;
        Vector3 targetScale = _originalScale * pulseScale;

        // 1. Увеличение (Zoom In)
        while (timer < duration / 2)
        {
            timer += Time.deltaTime;
            float progress = timer / (duration / 2);
            transform.localScale = Vector3.Lerp(_originalScale, targetScale, progress);
            yield return null;
        }

        // 2. Уменьшение (Zoom Out)
        timer = 0f;
        while (timer < duration / 2)
        {
            timer += Time.deltaTime;
            float progress = timer / (duration / 2);
            transform.localScale = Vector3.Lerp(targetScale, _originalScale, progress);
            yield return null;
        }

        // Гарантируем возврат к исходному размеру
        transform.localScale = _originalScale;
        _animationCoroutine = null;
    }

    private void OnDisable()
    {
        // Сброс при выключении объекта, чтобы он не остался увеличенным
        transform.localScale = _originalScale;
    }
}