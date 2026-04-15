using UnityEngine;

/// <summary>
/// 모바일 입력 통합 관리자. IInputProvider를 직접 구현.
/// 조이스틱 방향은 VirtualJoystick에서 읽고,
/// 점프는 Button의 onClick → OnJumpPressed()를 인스펙터에서 연결.
/// </summary>
public class MobileInputUI : MonoBehaviour, IInputProvider
{
    [SerializeField] private VirtualJoystick joystick;

    private bool _jumpLatched;

    public Vector2 MoveDirection => joystick != null ? joystick.Direction : Vector2.zero;

    /// <summary>
    /// 점프 버튼 Button 컴포넌트의 onClick에 인스펙터에서 연결.
    /// </summary>
    public void OnJumpPressed()
    {
        _jumpLatched = true;
    }

    public bool ConsumeJump()
    {
        if (!_jumpLatched) return false;
        _jumpLatched = false;
        return true;
    }
}
