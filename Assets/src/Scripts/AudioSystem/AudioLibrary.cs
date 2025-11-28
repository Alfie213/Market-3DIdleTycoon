using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Tycoon/Audio Library")]
public class AudioLibrary : ScriptableObject
{
    [Header("Music")]
    public AudioClip backgroundMusic;

    [Header("SFX")]
    public AudioClip coinSound;
    public AudioClip clickSound;
}