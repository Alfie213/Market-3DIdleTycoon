using UnityEngine;

/// <summary>
/// Persistent audio manager for Music and SFX.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioLibrary audioLibrary;

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    public bool IsMusicEnabled => !_musicSource.mute;
    public bool IsSFXEnabled => !_sfxSource.mute;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSources();
        }
        else Destroy(gameObject);
    }

    private void Start() => PlayMusic(audioLibrary.backgroundMusic);

    private void InitializeSources()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;

        _musicSource.mute = PlayerPrefs.GetInt(GameConstants.MusicMuteKey, 0) == 1;
        _sfxSource.mute = PlayerPrefs.GetInt(GameConstants.SfxMuteKey, 0) == 1;
    }

    public void SetMusicEnabled(bool isEnabled)
    {
        _musicSource.mute = !isEnabled;
        PlayerPrefs.SetInt(GameConstants.MusicMuteKey, !isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSFXEnabled(bool isEnabled)
    {
        _sfxSource.mute = !isEnabled;
        PlayerPrefs.SetInt(GameConstants.SfxMuteKey, !isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlayClickSound() => PlaySFX(audioLibrary.clickSound);
    public void PlayCoinSound() => PlaySFX(audioLibrary.coinSound);

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        
        // Добавляем вариативность питча (опционально), чтобы звук не был "роботизированным"
        _sfxSource.pitch = Random.Range(0.95f, 1.05f); 
        _sfxSource.PlayOneShot(clip);
        _sfxSource.pitch = 1f; // Возвращаем питч в норму
    }
}