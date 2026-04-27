using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Volume Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // 리스너는 Start에서 1회만 등록
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    public void OnGameStart()
    {
        SceneManager.LoadScene("Stage Select");
    }

    public void OnOpenSettings()
    {
        if (settingsPanel == null) return;

        // 패널 열릴 때 현재 볼륨값을 슬라이더에 반영
        if (bgmSlider != null && AudioManager.Instance != null)
            bgmSlider.SetValueWithoutNotify(AudioManager.Instance.BgmVolume);

        if (sfxSlider != null && AudioManager.Instance != null)
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.SfxVolume);

        settingsPanel.SetActive(true);
    }

    public void OnCloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void OnQuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnBgmVolumeChanged(float value)
    {
        AudioManager.Instance?.SetBGMVolume(value);
        SaveAudioSettings();
    }

    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
        SaveAudioSettings();
    }

    private void SaveAudioSettings()
    {
        if (AudioManager.Instance == null) return;
        StageProgressManager.Instance.SaveAudioSettings(
            AudioManager.Instance.BgmVolume,
            AudioManager.Instance.SfxVolume
        );
    }
}
