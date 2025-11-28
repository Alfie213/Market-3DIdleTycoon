using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveKey = "GameSave_V1";
    
    // Список всех объектов, которые нужно сохранить
    // Мы будем находить их автоматически или регистрировать вручную
    private List<ISaveable> _saveables = new List<ISaveable>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Проверяем по имени или индексу (лучше по Enum, но scene.name тоже пойдет)
        if (scene.name == "GameScene") 
        {
            // Ждем 1 кадр, чтобы все Start() методы на сцене успели зарегистрироваться
            StartCoroutine(LoadGameDelayed());
        }
    }
    
    public bool HasSaveData()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }
    
    public void DeleteSaveFile()
    {
        // Удаляем конкретный ключ сохранения
        PlayerPrefs.DeleteKey(SaveKey);
        
        // Или PlayerPrefs.DeleteAll(); // Если хочешь удалить вообще всё
        
        PlayerPrefs.Save();
        Debug.Log("Save file deleted.");
        
        // Опционально: перезагрузка игры после удаления
        // UnityEngine.SceneManagement.SceneManager.LoadScene("StartupScene");
    }

    private IEnumerator LoadGameDelayed()
    {
        yield return null; // Пропускаем кадр
        LoadGame();
    }
    
    public void RegisterSaveable(ISaveable saveable)
    {
        if (!_saveables.Contains(saveable))
            _saveables.Add(saveable);
    }

    public void UnregisterSaveable(ISaveable saveable)
    {
        if (_saveables.Contains(saveable))
            _saveables.Remove(saveable);
    }

    public void SaveGame()
    {
        GameSaveData data = new GameSaveData();

        // 1. Проходим по всем зарегистрированным объектам и просим их сохранить свои данные в data
        foreach (var saveable in _saveables)
        {
            saveable.PopulateSaveData(data);
        }

        // 2. Сериализуем в JSON
        string json = JsonUtility.ToJson(data, true); // true для красивого форматирования
        
        // 3. Пишем на диск (в PlayerPrefs)
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            Debug.Log("No save file found. Starting new game.");
            return;
        }

        string json = PlayerPrefs.GetString(SaveKey);
        
        try 
        {
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

            // Раздаем данные всем подписчикам
            foreach (var saveable in _saveables)
            {
                saveable.LoadFromSaveData(data);
            }
            
            Debug.Log("Game Loaded!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load save: {e.Message}");
        }
    }
    
    // Для дебага: очистка сохранения
    [ContextMenu("Delete Save")]
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        Debug.Log("Save Deleted");
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}