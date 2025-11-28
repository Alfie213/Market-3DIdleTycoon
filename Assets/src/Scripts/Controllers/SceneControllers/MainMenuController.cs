using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button loadGameButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI loadGameButtonText;

    private void Start()
    {
        // 1. Настраиваем кнопку "Load Game"
        if (SaveManager.Instance != null && SaveManager.Instance.HasSaveData())
        {
            // Сохранение есть: кнопка активна, текст по умолчанию (как в инспекторе)
            loadGameButton.interactable = true;
            loadGameButton.onClick.AddListener(OnLoadGameClicked);
        }
        else
        {
            // Сохранения нет: выключаем кнопку, меняем текст
            loadGameButton.interactable = false;
            
            // Используем теги TMP для форматирования: заголовок покрупнее, пояснение помельче
            loadGameButtonText.text = "No Save Data\n<size=50%>Game saves automatically on exit</size>";
        }

        // 2. Настраиваем кнопку "Start Game"
        startGameButton.onClick.AddListener(OnStartGameClicked);
    }

    private void OnStartGameClicked()
    {
        // Новая игра = удаляем старые данные
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSaveFile();
        }

        // Грузим сцену (SaveManager не найдет файл и инициализирует игру с нуля)
        SceneLoader.Instance.LoadScene(SceneType.GameScene);
    }

    private void OnLoadGameClicked()
    {
        // Продолжить игру = просто грузим сцену
        // SaveManager сам найдет файл и загрузит данные
        SceneLoader.Instance.LoadScene(SceneType.GameScene);
    }
}