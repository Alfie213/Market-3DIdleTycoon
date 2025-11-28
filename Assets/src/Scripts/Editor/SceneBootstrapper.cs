using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class SceneBootstrapper
{
    private const string MenuPath = "Tools/Always Start From Bootstrapper";
    private const string SettingKey = "SceneBootstrapper_Enabled";

    static SceneBootstrapper()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            RefreshPlayModeStartScene();
        }
    }

    [MenuItem(MenuPath)]
    private static void ToggleAction()
    {
        bool isEnabled = EditorPrefs.GetBool(SettingKey, true);
        EditorPrefs.SetBool(SettingKey, !isEnabled);
        
        // Force refresh GUI checkmark
        Menu.SetChecked(MenuPath, !isEnabled);
        
        RefreshPlayModeStartScene();
    }

    [MenuItem(MenuPath, true)]
    private static bool ValidateToggleAction()
    {
        Menu.SetChecked(MenuPath, EditorPrefs.GetBool(SettingKey, true));
        return true;
    }

    private static void RefreshPlayModeStartScene()
    {
        bool isEnabled = EditorPrefs.GetBool(SettingKey, true);

        if (isEnabled)
        {
            if (EditorBuildSettings.scenes.Length == 0)
            {
                Debug.LogWarning("Build Settings are empty! Cannot start from Startup Scene.");
                return;
            }

            string scenePath = EditorBuildSettings.scenes[0].path;
            
            if (!string.IsNullOrEmpty(scenePath))
            {
                SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                EditorSceneManager.playModeStartScene = sceneAsset;
            }
            else
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }
        else
        {
            EditorSceneManager.playModeStartScene = null;
        }
    }
}