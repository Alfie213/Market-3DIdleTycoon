using System.Collections;
using UnityEngine;

public class StartupController : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Небольшая задержка перед началом загрузки меню (опционально),
        // чтобы инициализация систем прошла успешно.
        yield return null; 

        SceneLoader.Instance.LoadScene(SceneType.MainMenuScene);
    }
}