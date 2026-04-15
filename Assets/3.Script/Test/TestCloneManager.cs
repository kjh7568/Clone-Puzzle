using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// TestScene_ClonePlayback 씬 전용 임시 테스트 매니저.
/// WASD(A/D 이동, W 점프)로 플레이어 입력을 주입하고,
/// SpaceBar로 녹화를 종료한 뒤 클론에 재생을 넘긴다.
/// R키로 씬 전체를 리셋한다.
/// </summary>
public class TestCloneManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputRecorder inputRecorder;
    [SerializeField] private ClonePlayback clonePlayback;
    [SerializeField] private Transform playerTransform;

    private Vector3 _playerStartPosition;
    private bool _isRecording;

    private void Start()
    {
        _playerStartPosition = playerTransform.position;

        clonePlayback.gameObject.SetActive(false);
        clonePlayback.OnPlaybackFinished.AddListener(OnClonePlaybackFinished);

        inputRecorder.StartRecording();
        _isRecording = true;

        Debug.Log("[TestCloneManager] 녹화 시작 | Space: 녹화 종료 & 클론 재생  R: 씬 리셋");
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (kb.spaceKey.wasPressedThisFrame && _isRecording)
        {
            StopAndPlayClone();
        }
    }

    // ── 녹화 종료 & 클론 재생 ────────────────────────────────────────────

    private void StopAndPlayClone()
    {
        _isRecording = false;
        inputRecorder.StopRecording();

        List<FrameInput> recordedData = inputRecorder.GetRecordedData();
        Debug.Log($"[TestCloneManager] 녹화 종료 — {recordedData.Count} 프레임 캡처");

        clonePlayback.gameObject.SetActive(true);
        clonePlayback.transform.position = _playerStartPosition;
        clonePlayback.Play(recordedData);

        Debug.Log("[TestCloneManager] 클론 재생 시작 / 플레이어 시작 위치 리셋");
    }

    // ── 이벤트 콜백 ──────────────────────────────────────────────────────

    private void OnClonePlaybackFinished()
    {
        Debug.Log("[TestCloneManager] 클론 재생 완료");
    }

    private void OnDestroy()
    {
        if (clonePlayback != null)
            clonePlayback.OnPlaybackFinished.RemoveListener(OnClonePlaybackFinished);
    }
}
