using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseWindowRoot; // Само окно с кнопками
    [SerializeField] private GameObject blackoutObject;  // Полупрозрачный черный фон
    
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;
    
    [Header("Settings Toggles")]
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    private bool _isPaused = false;

    private void Start()
    {
        // 1. Скрываем меню и затемнение при старте
        pauseWindowRoot.SetActive(false);
        if (blackoutObject != null) blackoutObject.SetActive(false);

        // 2. Настраиваем кнопки
        resumeButton.onClick.AddListener(TogglePause);
        exitButton.onClick.AddListener(ExitToMainMenu);

        // 3. Настраиваем Тоглы
        InitializeSettings();
    }

    private void Update()
    {
        // Кнопка вызова паузы (можно добавить кнопку на экран, которая будет вызывать TogglePause)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            ActivatePause();
        }
        else
        {
            DeactivatePause();
        }
    }

    private void ActivatePause()
    {
        Time.timeScale = 0f;
        pauseWindowRoot.SetActive(true);
        
        // Включаем затемнение
        if (blackoutObject != null) blackoutObject.SetActive(true);
    }

    private void DeactivatePause()
    {
        Time.timeScale = 1f;
        pauseWindowRoot.SetActive(false);
        
        // Выключаем затемнение
        if (blackoutObject != null) blackoutObject.SetActive(false);
    }

    private void ExitToMainMenu()
    {
        Time.timeScale = 1f;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }

        SceneLoader.Instance.LoadScene(SceneType.MainMenuScene);
    }

    private void InitializeSettings()
    {
        if (AudioManager.Instance == null)
        {
            // Если тестируем сцену без менеджера, просто выключаем управление звуком
            musicToggle.interactable = false;
            sfxToggle.interactable = false;
            return;
        }

        musicToggle.isOn = AudioManager.Instance.IsMusicEnabled;
        sfxToggle.isOn = AudioManager.Instance.IsSFXEnabled;

        musicToggle.onValueChanged.AddListener((isEnabled) => 
        {
            AudioManager.Instance.SetMusicEnabled(isEnabled);
        });

        sfxToggle.onValueChanged.AddListener((isEnabled) => 
        {
            AudioManager.Instance.SetSFXEnabled(isEnabled);
        });
    }
}