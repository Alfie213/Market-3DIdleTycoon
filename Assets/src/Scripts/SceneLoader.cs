using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SceneType
{
    MainMenuScene,
    GameScene
}

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject loadingScreenRoot; // Весь Canvas загрузки
    [SerializeField] private Slider progressBar; // Слайдер прогресса

    [Header("Settings")]
    [SerializeField] private float minLoadTime = 1.0f; // Чтобы экран загрузки не мелькал слишком быстро

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Сразу скрываем экран загрузки при инициализации, если он вдруг включен
            if (loadingScreenRoot) loadingScreenRoot.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(SceneType scene)
    {
        StartCoroutine(LoadSceneRoutine(scene));
    }

    private IEnumerator LoadSceneRoutine(SceneType scene)
    {
        // 1. Включаем экран загрузки
        if (loadingScreenRoot) loadingScreenRoot.SetActive(true);
        if (progressBar) progressBar.value = 0;

        string sceneName = scene.ToString();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        // Отключаем авто-переключение, чтобы контролировать процесс
        operation.allowSceneActivation = false;

        float timer = 0f;
        
        // 2. Крутим цикл пока не загрузится
        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            // Unity грузит сцену до 0.9, потом ждет allowSceneActivation
            // Нормализуем прогресс: (0.9 -> 1.0)
            float currentProgress = Mathf.Clamp01(operation.progress / 0.9f);
            
            if (progressBar) progressBar.value = currentProgress;

            // Ждем, пока пройдет минимальное время И загрузка дойдет до точки активации
            if (operation.progress >= 0.9f && timer >= minLoadTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // 3. Выключаем экран загрузки после полной загрузки новой сцены
        if (loadingScreenRoot) loadingScreenRoot.SetActive(false);
    }
}