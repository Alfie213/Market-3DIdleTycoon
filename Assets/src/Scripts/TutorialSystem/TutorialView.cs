using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialView : MonoBehaviour
{
    [SerializeField] private GameObject contentRoot; // Панель с фоном
    [SerializeField] private TextMeshProUGUI tutorialText;
    
    public void Show(string text)
    {
        contentRoot.SetActive(true);
        tutorialText.text = text;
    }

    public void Hide()
    {
        contentRoot.SetActive(false);
    }
    
    // Вспомогательный метод для автоматического скрытия через время
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