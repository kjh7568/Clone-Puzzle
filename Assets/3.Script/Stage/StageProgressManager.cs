using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StageProgressManager : MonoBehaviour
{
    private static readonly string SavePath =
        Application.persistentDataPath + "/savedata.json";

    private static StageProgressManager _instance;

    public static StageProgressManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("StageProgressManager");
                _instance = go.AddComponent<StageProgressManager>();
            }
            return _instance;
        }
    }

    private HashSet<int> _clearedStages = new HashSet<int>();

    public float BgmVolume { get; private set; } = 0.7f;
    public float SfxVolume { get; private set; } = 1f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    // ── 스테이지 진행도 ────────────────────────────────────────────────────

    public void MarkCleared(int stageNumber)
    {
        if (stageNumber < 1) return;
        _clearedStages.Add(stageNumber);
        Save();
        Debug.Log($"[StageProgressManager] Stage {stageNumber} cleared & saved.");
    }

    public bool IsCleared(int stageNumber)
    {
        return _clearedStages.Contains(stageNumber);
    }

    public bool IsUnlocked(int stageNumber)
    {
        if (stageNumber == 1) return true;
        return IsCleared(stageNumber - 1);
    }

    public static int ParseStageNumber(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return -1;
        var parts = sceneName.Split(' ');
        if (parts.Length >= 2 && int.TryParse(parts[parts.Length - 1], out int n))
            return n;
        return -1;
    }

    // ── 오디오 설정 ───────────────────────────────────────────────────────

    public void SaveAudioSettings(float bgmVolume, float sfxVolume)
    {
        BgmVolume = Mathf.Clamp01(bgmVolume);
        SfxVolume = Mathf.Clamp01(sfxVolume);
        Save();
    }

    // ── 앱 생명주기 ───────────────────────────────────────────────────────

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) Save();
    }

    // ── 저장 / 로드 ───────────────────────────────────────────────────────

    private void Load()
    {
        if (!File.Exists(SavePath))
        {
            _clearedStages = new HashSet<int>();
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<SaveData>(json);
            _clearedStages = new HashSet<int>(data.clearedStages);
            BgmVolume = data.bgmVolume;
            SfxVolume = data.sfxVolume;
        }
        catch
        {
            Debug.LogWarning("[StageProgressManager] 세이브 파일 로드 실패. 초기화합니다.");
            _clearedStages = new HashSet<int>();
        }
    }

    private void Save()
    {
        var data = new SaveData();
        data.clearedStages.AddRange(_clearedStages);
        data.bgmVolume = BgmVolume;
        data.sfxVolume = SfxVolume;

        try
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(SavePath, json);
        }
        catch
        {
            Debug.LogWarning("[StageProgressManager] 세이브 파일 저장 실패.");
        }
    }
}
