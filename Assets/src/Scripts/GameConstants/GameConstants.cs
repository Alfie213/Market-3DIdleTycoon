using UnityEngine;

/// <summary>
/// Centralized storage for constants, keys, and IDs to avoid magic strings/numbers.
/// </summary>
public static class GameConstants
{
    // PlayerPrefs Keys
    public const string SaveKey = "GameSave_V1";
    public const string MusicMuteKey = "MusicMute";
    public const string SfxMuteKey = "SFXMute";

    // Animation Parameters
    public const string AnimParamIsMoving = "IsMoving";

    // Scene Names (Must match Build Settings)
    public const string SceneStartup = "StartupScene";
    public const string SceneMainMenu = "MainMenuScene";
    public const string SceneGame = "GameScene";
}