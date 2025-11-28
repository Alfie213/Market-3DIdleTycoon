using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton manager that handles JSON serialization/deserialization of game data.
/// Persists data to PlayerPrefs.
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private readonly List<ISaveable> _saveables = new List<ISaveable>();

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

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    /// <summary>
    /// Automatically attempts to load the game when the Gameplay scene starts.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            StartCoroutine(LoadGameDelayed());
        }
    }

    private IEnumerator LoadGameDelayed()
    {
        // Wait one frame to ensure all Start() methods on the scene have run and registered their Saveables.
        yield return null; 
        LoadGame();
    }

    /// <summary>
    /// Registers an object to be included in the save/load process.
    /// Should be called in Start().
    /// </summary>
    public void RegisterSaveable(ISaveable saveable)
    {
        if (!_saveables.Contains(saveable)) _saveables.Add(saveable);
    }

    public void UnregisterSaveable(ISaveable saveable)
    {
        if (_saveables.Contains(saveable)) _saveables.Remove(saveable);
    }

    /// <summary>
    /// Collects data from all registered objects and writes it to PlayerPrefs.
    /// </summary>
    public void SaveGame()
    {
        GameSaveData data = new GameSaveData();
        foreach (var saveable in _saveables)
        {
            saveable.PopulateSaveData(data);
        }

        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(GameConstants.SaveKey, json);
        PlayerPrefs.Save();
        
        Debug.Log("Game Saved");
    }

    /// <summary>
    /// Reads JSON from PlayerPrefs and distributes data to all registered objects.
    /// </summary>
    public void LoadGame()
    {
        if (!HasSaveData()) return;

        try 
        {
            string json = PlayerPrefs.GetString(GameConstants.SaveKey);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

            foreach (var saveable in _saveables)
            {
                saveable.LoadFromSaveData(data);
            }
            Debug.Log("Game Loaded");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load save: {e.Message}");
        }
    }

    public bool HasSaveData() => PlayerPrefs.HasKey(GameConstants.SaveKey);

    public void DeleteSaveFile()
    {
        PlayerPrefs.DeleteKey(GameConstants.SaveKey);
        PlayerPrefs.Save();
        Debug.Log("Save file deleted");
    }

    // Auto-save on mobile minimize or PC quit
    private void OnApplicationPause(bool pauseStatus) { if (pauseStatus) SaveGame(); }
    private void OnApplicationQuit() { SaveGame(); }
}