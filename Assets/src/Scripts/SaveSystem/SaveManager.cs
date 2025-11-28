using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages data persistence using PlayerPrefs and JSON.
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
        else Destroy(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == GameConstants.SceneGame)
        {
            StartCoroutine(LoadGameDelayed());
        }
    }

    private IEnumerator LoadGameDelayed()
    {
        yield return null; 
        LoadGame();
    }

    public void RegisterSaveable(ISaveable saveable)
    {
        if (!_saveables.Contains(saveable)) _saveables.Add(saveable);
    }

    public void UnregisterSaveable(ISaveable saveable)
    {
        if (_saveables.Contains(saveable)) _saveables.Remove(saveable);
    }

    public void SaveGame()
    {
        GameSaveData data = new GameSaveData();
        foreach (var saveable in _saveables) saveable.PopulateSaveData(data);

        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(GameConstants.SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (!HasSaveData()) return;
        try 
        {
            string json = PlayerPrefs.GetString(GameConstants.SaveKey);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
            foreach (var saveable in _saveables) saveable.LoadFromSaveData(data);
            Debug.Log("Game Loaded");
        }
        catch (System.Exception e) { Debug.LogError($"Failed to load save: {e.Message}"); }
    }

    public bool HasSaveData() => PlayerPrefs.HasKey(GameConstants.SaveKey);

    public void DeleteSaveFile()
    {
        PlayerPrefs.DeleteKey(GameConstants.SaveKey);
        PlayerPrefs.Save();
        Debug.Log("Save file deleted");
    }

    private void OnApplicationPause(bool pauseStatus) { if (pauseStatus) SaveGame(); }
    private void OnApplicationQuit() { SaveGame(); }
}