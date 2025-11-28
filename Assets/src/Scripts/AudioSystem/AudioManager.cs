using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioLibrary audioLibrary;

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    private const string MusicMuteKey = "MusicMute";
    private const string SFXMuteKey = "SFXMute";

    // Публичные свойства для UI (чтобы знать, ставить галочку или нет)
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
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Запускаем музыку сразу при старте игры
        PlayMusic(audioLibrary.backgroundMusic);
    }

    private void InitializeSources()
    {
        // Создаем источники звука программно, чтобы не настраивать их на сцене
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;

        // Загружаем сохраненные настройки
        _musicSource.mute = PlayerPrefs.GetInt(MusicMuteKey, 0) == 1;
        _sfxSource.mute = PlayerPrefs.GetInt(SFXMuteKey, 0) == 1;
    }

    // --- ПУБЛИЧНЫЕ МЕТОДЫ ДЛЯ UI ---

    public void SetMusicEnabled(bool isEnabled)
    {
        _musicSource.mute = !isEnabled;
        PlayerPrefs.SetInt(MusicMuteKey, !isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetSFXEnabled(bool isEnabled)
    {
        _sfxSource.mute = !isEnabled;
        PlayerPrefs.SetInt(SFXMuteKey, !isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    // --- ЛОГИКА ВОСПРОИЗВЕДЕНИЯ ---

    public void PlayClickSound()
    {
        PlaySFX(audioLibrary.clickSound);
    }

    public void PlayCoinSound()
    {
        // Используем PlayOneShot, чтобы звуки могли накладываться друг на друга
        // (например, если много продаж одновременно)
        PlaySFX(audioLibrary.coinSound);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip);
    }
}