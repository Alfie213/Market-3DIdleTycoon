using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Persistent singleton handling scene transitions and loading screens.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    public enum SceneType { StartupScene, MainMenuScene, GameScene }

    [Header("UI References")]
    [SerializeField] private GameObject loadingScreenRoot;
    [SerializeField] private Slider progressBar;

    [Header("Settings")]
    [SerializeField] private float minLoadTime = 1.0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (loadingScreenRoot) loadingScreenRoot.SetActive(false);
        }
        else Destroy(gameObject);
    }

    public void LoadScene(SceneType scene) => StartCoroutine(LoadSceneRoutine(scene));

    private IEnumerator LoadSceneRoutine(SceneType scene)
    {
        if (loadingScreenRoot) loadingScreenRoot.SetActive(true);
        if (progressBar) progressBar.value = 0;

        string sceneName = scene.ToString();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float timer = 0f;
        while (!operation.isDone)
        {
            timer += Time.deltaTime;
            float currentProgress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar) progressBar.value = currentProgress;

            if (operation.progress >= 0.9f && timer >= minLoadTime)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        if (loadingScreenRoot) loadingScreenRoot.SetActive(false);
    }
}