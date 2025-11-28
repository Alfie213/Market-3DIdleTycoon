using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pauseWindowRoot;
    [SerializeField] private GameObject blackoutObject;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    private bool _isPaused = false;

    private void Start()
    {
        pauseWindowRoot.SetActive(false);
        if (blackoutObject != null) blackoutObject.SetActive(false);

        resumeButton.onClick.AddListener(TogglePause);
        exitButton.onClick.AddListener(ExitToMainMenu);

        InitializeSettings();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        if (_isPaused) ActivatePause();
        else DeactivatePause();
    }

    private void ActivatePause()
    {
        Time.timeScale = 0f;
        pauseWindowRoot.SetActive(true);
        if (blackoutObject != null) blackoutObject.SetActive(true);
    }

    private void DeactivatePause()
    {
        Time.timeScale = 1f;
        pauseWindowRoot.SetActive(false);
        if (blackoutObject != null) blackoutObject.SetActive(false);
    }

    private void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        if (SaveManager.Instance != null) SaveManager.Instance.SaveGame();
        SceneLoader.Instance.LoadScene(SceneLoader.SceneType.MainMenuScene);
    }

    private void InitializeSettings()
    {
        if (AudioManager.Instance == null)
        {
            musicToggle.interactable = false;
            sfxToggle.interactable = false;
            return;
        }

        musicToggle.isOn = AudioManager.Instance.IsMusicEnabled;
        sfxToggle.isOn = AudioManager.Instance.IsSFXEnabled;

        musicToggle.onValueChanged.AddListener((isEnabled) => AudioManager.Instance.SetMusicEnabled(isEnabled));
        sfxToggle.onValueChanged.AddListener((isEnabled) => AudioManager.Instance.SetSFXEnabled(isEnabled));
    }
}