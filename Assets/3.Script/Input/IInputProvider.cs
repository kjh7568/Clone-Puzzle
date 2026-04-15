using UnityEngine;

/// <summary>
/// 플레이어/클론 공통 입력 추상화 인터페이스.
/// PlayerController는 조이스틱/버튼에서, ClonePlayback은 녹화 데이터에서 구현.
/// </summary>
public interface IInputProvider
{
    /// <summary>수평 이동 방향 (-1 ~ 1, x축만 사용)</summary>
    Vector2 MoveDirection { get; }

    /// <summary>점프 입력. FixedUpdate에서 소비(Consume)되면 자동 해제.</summary>
    bool ConsumeJump();

    /// <summary>인터랙트 입력. 소비 후 자동 해제.</summary>
    bool ConsumeInteract();
}
