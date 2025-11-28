using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Tycoon/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [Header("Music")]
    public AudioClip backgroundMusic;

    [Header("SFX")]
    public AudioClip coinSound; // Звук получения денег
    public AudioClip clickSound; // Звук нажатия кнопки
}