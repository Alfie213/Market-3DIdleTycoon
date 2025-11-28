using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            musicToggle.isOn = AudioManager.Instance.IsMusicEnabled;
            sfxToggle.isOn = AudioManager.Instance.IsSFXEnabled;
        }

        musicToggle.onValueChanged.AddListener(OnMusicToggled);
        sfxToggle.onValueChanged.AddListener(OnSFXToggled);
    }

    private void OnMusicToggled(bool isEnabled)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicEnabled(isEnabled);
        }
    }

    private void OnSFXToggled(bool isEnabled)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXEnabled(isEnabled);
        }
    }
}