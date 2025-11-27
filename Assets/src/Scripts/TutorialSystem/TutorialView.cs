using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialView : MonoBehaviour
{
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private TextMeshProUGUI tutorialText;
    
    // Ссылка на анимацию (добавь скрипт на contentRoot или на саму панель)
    [SerializeField] private UIPulseAnimation pulseAnimation;

    public void Show(string text)
    {
        contentRoot.SetActive(true);
        tutorialText.text = text;

        // Запускаем пульсацию
        if (pulseAnimation != null)
        {
            pulseAnimation.Play();
        }
    }

    public void Hide()
    {
        contentRoot.SetActive(false);
    }
    
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