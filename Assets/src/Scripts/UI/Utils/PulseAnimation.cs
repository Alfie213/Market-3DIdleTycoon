using System.Collections;
using UnityEngine;

/// <summary>
/// Generic simple animation (Zoom In/Out) for UI or 3D objects.
/// </summary>
public class PulseAnimation : MonoBehaviour
{
    [SerializeField] private float pulseScale = 1.1f;
    [SerializeField] private float duration = 0.15f;

    private Vector3 _originalScale;
    private Coroutine _animationCoroutine;

    private void Awake() => _originalScale = transform.localScale;
    private void OnDisable() => transform.localScale = _originalScale;

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

        while (timer < duration / 2)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(_originalScale, targetScale, timer / (duration / 2));
            yield return null;
        }

        timer = 0f;
        while (timer < duration / 2)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, _originalScale, timer / (duration / 2));
            yield return null;
        }
        transform.localScale = _originalScale;
    }
}