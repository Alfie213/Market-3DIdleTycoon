using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private TextMeshProUGUI loadGameButtonText;

    private void Start()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.HasSaveData())
        {
            loadGameButton.interactable = true;
            loadGameButton.onClick.AddListener(() => SceneLoader.Instance.LoadScene(SceneLoader.SceneType.GameScene));
        }
        else
        {
            loadGameButton.interactable = false;
            loadGameButtonText.text = "No Save Data\n<size=50%>Game saves automatically on exit</size>";
        }

        startGameButton.onClick.AddListener(OnStartGameClicked);
    }

    private void OnStartGameClicked()
    {
        if (SaveManager.Instance != null) SaveManager.Instance.DeleteSaveFile();
        SceneLoader.Instance.LoadScene(SceneLoader.SceneType.GameScene);
    }
}