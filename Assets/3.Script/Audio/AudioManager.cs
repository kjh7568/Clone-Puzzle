using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("AudioManager");
                _instance = go.AddComponent<AudioManager>();
            }
            return _instance;
        }
    }

    [SerializeField] private AudioData audioData;

    [Header("Volume")]
    [Range(0f, 1f)] [SerializeField] private float bgmVolume = 0.7f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    private AudioSource _bgmSource;
    private AudioSource _sfxSource;
    private BgmType _currentBgm = BgmType.None;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // 이미 붙어있는 AudioSource 재사용, 없으면 추가
        var existingSources = GetComponents<AudioSource>();
        _bgmSource = existingSources.Length > 0 ? existingSources[0] : gameObject.AddComponent<AudioSource>();
        _sfxSource = existingSources.Length > 1 ? existingSources[1] : gameObject.AddComponent<AudioSource>();

        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _sfxSource.playOnAwake = false;

        // 저장된 볼륨 적용 (파일 없으면 인스펙터 기본값 유지)
        var progress = StageProgressManager.Instance;
        bgmVolume = progress.BgmVolume;
        sfxVolume = progress.SfxVolume;
        _bgmSource.volume = bgmVolume;
        _sfxSource.volume = sfxVolume;
    }

    public void PlayBGM(BgmType type)
    {
        if (type == BgmType.None) { StopBGM(); return; }
        if (_currentBgm == type && _bgmSource.isPlaying) return;

        AudioClip clip = type switch
        {
            BgmType.Stage       => audioData?.bgmStage,
            BgmType.Menu        => audioData?.bgmMenu,
            BgmType.StageSelect => audioData?.bgmStageSelect,
            _                   => null,
        };

        if (clip == null) { Debug.LogWarning($"[AudioManager] BGM 클립 없음: {type}"); return; }

        _bgmSource.Stop();
        _currentBgm = type;
        _bgmSource.clip = clip;
        _bgmSource.Play();
        Debug.Log($"[AudioManager] BGM: {type}");
    }

    public void StopBGM()
    {
        _currentBgm = BgmType.None;
        _bgmSource.Stop();
    }

    public void PlaySFX(SfxType type)
    {
        AudioClip clip = type switch
        {
            SfxType.JumpLand     => audioData?.sfxJumpLand,
            SfxType.CarryPickup  => audioData?.sfxCarryPickup,
            SfxType.CarryDrop    => audioData?.sfxCarryDrop,
            SfxType.ButtonClick  => audioData?.sfxButtonClick,
            SfxType.ButtonPress  => audioData?.sfxButtonPress,
            SfxType.CloneCreate  => audioData?.sfxCloneCreate,
            SfxType.CloneEnd     => audioData?.sfxCloneEnd,
            SfxType.CloneReset   => audioData?.sfxCloneReset,
            SfxType.StageCleared => audioData?.sfxStageCleared,
            SfxType.DoorOpen     => audioData?.sfxDoorOpen,
            SfxType.ScaleBalance => audioData?.sfxScaleBalance,
            _                    => null,
        };

        if (clip == null) { Debug.LogWarning($"[AudioManager] SFX 클립 없음: {type}"); return; }

        _sfxSource.PlayOneShot(clip, sfxVolume);
        Debug.Log($"[AudioManager] SFX: {type}");
    }

    public float BgmVolume => bgmVolume;
    public float SfxVolume => sfxVolume;

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        _bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        _sfxSource.volume = sfxVolume;
    }

    private void OnValidate()
    {
        if (_bgmSource != null) _bgmSource.volume = bgmVolume;
        if (_sfxSource != null) _sfxSource.volume = sfxVolume;
    }
}
