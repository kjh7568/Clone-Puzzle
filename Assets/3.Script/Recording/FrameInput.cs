using UnityEngine;

/// <summary>
/// 한 프레임의 입력 상태 스냅샷.
/// InputRecorder가 매 FixedUpdate마다 기록하고,
/// ClonePlayback이 순서대로 읽어 IInputProvider로 재생한다.
/// </summary>
public struct FrameInput
{
    /// <summary>녹화 시작 기준 경과 시간 (초)</summary>
    public float timestamp;

    /// <summary>수평 이동 방향 (x축만 사용)</summary>
    public Vector2 moveDirection;

    /// <summary>이 프레임에 점프 버튼이 눌렸는지 여부</summary>
    public bool jumpPressed;

    /// <summary>이 프레임에 인터랙트 버튼이 눌렸는지 여부</summary>
    public bool interactPressed;
}
