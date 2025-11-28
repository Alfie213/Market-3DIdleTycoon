using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialView : MonoBehaviour, IView
{
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private PulseAnimation pulseAnimation;

    public void Show(string text)
    {
        contentRoot.SetActive(true);
        tutorialText.text = text;
        if (pulseAnimation != null) pulseAnimation.Play();
    }

    public void Show() => contentRoot.SetActive(true);
    public void Hide() => contentRoot.SetActive(false);

    public void ShowAndHideDelayed(string text, float delay)
    {
        Show(text);
        StopAllCoroutines();
        StartCoroutine(HideRoutine(delay));
    }

    private IEnumerator HideRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Hide();
    }
}