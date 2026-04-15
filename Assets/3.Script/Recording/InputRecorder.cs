using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모바일 입력 레이어에서 주입받은 입력을 IInputProvider로 Actor에 전달하면서,
/// 동시에 매 FixedUpdate마다 FrameInput으로 캡처해 녹화하는 컴포넌트.
/// ClonePlayback이 GetRecordedData()로 녹화 데이터를 수집해 재생한다.
/// </summary>
public class InputRecorder : MonoBehaviour, IInputProvider, IRecordable
{
    [SerializeField] private float maxRecordTime = 30f;

    // ── 입력 상태 (외부에서 SetInput으로 주입) ──────────────────────────
    private Vector2 _moveDirection;
    private bool _jumpPressed;
    private bool _interactPressed;

    // ── 녹화 상태 ────────────────────────────────────────────────────────
    private List<FrameInput> _recordedFrames = new();
    private float _recordStartTime;
    private bool _isRecording;

    // ── IInputProvider ───────────────────────────────────────────────────

    public Vector2 MoveDirection => _moveDirection;

    public bool ConsumeJump()
    {
        bool value = _jumpPressed;
        _jumpPressed = false;
        return value;
    }

    public bool ConsumeInteract()
    {
        bool value = _interactPressed;
        _interactPressed = false;
        return value;
    }

    // ── IRecordable ──────────────────────────────────────────────────────

    public void StartRecording()
    {
        _recordedFrames.Clear();
        _recordStartTime = Time.time;
        _isRecording = true;
    }

    public void StopRecording()
    {
        _isRecording = false;
    }

    public List<FrameInput> GetRecordedData()
    {
        return _recordedFrames;
    }

    // ── 외부 입력 주입 ───────────────────────────────────────────────────

    /// <summary>
    /// 모바일 입력 레이어(MobileInputUI 등)에서 매 프레임 호출.
    /// jump·interact는 누른 순간만 true로 전달해야 한다.
    /// </summary>
    public void SetInput(Vector2 move, bool jump, bool interact)
    {
        _moveDirection = move;
        if (jump) _jumpPressed = true;       // OR-accumulate: 놓치지 않도록
        if (interact) _interactPressed = true;
    }

    // ── 녹화 루프 ────────────────────────────────────────────────────────

    private void FixedUpdate()
    {
        if (!_isRecording) return;

        float elapsed = Time.time - _recordStartTime;

        if (elapsed >= maxRecordTime)
        {
            StopRecording();
            return;
        }

        _recordedFrames.Add(new FrameInput
        {
            timestamp      = elapsed,
            moveDirection  = _moveDirection,
            jumpPressed    = _jumpPressed,
            interactPressed = _interactPressed,
        });
    }
}
