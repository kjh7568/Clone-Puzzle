using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 클론 생성·리셋·재생을 총괄하는 매니저.
/// UI 버튼의 onClick에 OnCreateClone / OnEndCreation을 연결한다.
///
/// 라운드 흐름:
///   OnCreateClone() → 씬 리셋 + 녹화 시작
///   OnEndCreation()  → 녹화 종료 + 씬 리셋 + 전체 클론 동시 재생
/// </summary>
public class CloneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputRecorder inputRecorder;
    [SerializeField] private ClonePlayback clonePrefab;
    [SerializeField] private Transform cloneSpawnPoint;

    private readonly List<List<FrameInput>> _allRecordedData = new();
    private readonly List<ClonePlayback> _activeClones = new();
    private IResettable[] _resettables;
    private bool _isRecording;

    private void Start()
    {
        // IResettable을 구현한 씬 내 모든 MonoBehaviour 수집
        _resettables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IResettable>()
            .ToArray();

        foreach (var r in _resettables)
            r.SaveInitialState();

        Debug.Log($"[CloneManager] 초기화 완료 — IResettable {_resettables.Length}개 등록");
    }

    // ── 버튼 콜백 ────────────────────────────────────────────────────────

    /// <summary>
    /// "클론 생성하기" 버튼 onClick에 연결.
    /// 씬을 리셋하고 플레이어 녹화를 시작한다.
    /// </summary>
    public void OnCreateClone()
    {
        if (_isRecording) return;

        ResetAll();
        inputRecorder.StartRecording();
        _isRecording = true;

        Debug.Log($"[CloneManager] 녹화 시작 | 현재 클론 수: {_allRecordedData.Count}");
    }

    /// <summary>
    /// "생성 종료하기" 버튼 onClick에 연결.
    /// 녹화를 종료하고 씬을 리셋한 뒤 전체 클론을 동시에 재생한다.
    /// </summary>
    public void OnEndCreation()
    {
        if (!_isRecording) return;

        _isRecording = false;
        inputRecorder.StopRecording();

        // 녹화 데이터 복사 저장 (InputRecorder 버퍼 재사용 방지)
        _allRecordedData.Add(new List<FrameInput>(inputRecorder.GetRecordedData()));

        Debug.Log($"[CloneManager] 녹화 종료 — {_allRecordedData[^1].Count}프레임 | 총 클론 {_allRecordedData.Count}개");

        ResetAll();
    }

    // ── 내부 ─────────────────────────────────────────────────────────────

    private void ResetAll()
    {
        // 1. 클론이 들고 있는 오브젝트를 먼저 분리 (Destroy 전에 해야 CarryBox가 살아남음)
        foreach (var clone in _activeClones)
        {
            if (clone != null && clone.CarriedObject != null)
                clone.CarriedObject.OnPutDown(clone.transform.position);
        }

        // 2. 기존 클론 Destroy
        foreach (var clone in _activeClones)
            if (clone != null) Destroy(clone.gameObject);
        _activeClones.Clear();

        // 3. 모든 IResettable 초기 상태 복원 (이벤트 미발행)
        foreach (var r in _resettables)
            r.ResetState();

        // 4. 녹화된 클론 수만큼 Instantiate 후 즉시 재생
        foreach (var data in _allRecordedData)
        {
            ClonePlayback clone = Instantiate(clonePrefab, cloneSpawnPoint.position, Quaternion.identity);
            clone.Play(data);
            _activeClones.Add(clone);
        }

        Debug.Log($"[CloneManager] 씬 리셋 완료 | 재생 중인 클론: {_activeClones.Count}개");
    }
}
