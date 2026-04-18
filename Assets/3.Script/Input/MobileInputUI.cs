using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 모바일 입력 통합 관리자. IInputProvider를 직접 구현.
/// 조이스틱 방향은 VirtualJoystick에서 읽고,
/// 버튼 onClick을 인스펙터에서 각 메서드에 연결한다.
/// </summary>
public class MobileInputUI : MonoBehaviour, IInputProvider
{
    [SerializeField] private VirtualJoystick joystick;

    [Header("Clone Buttons")]
    [SerializeField] private Button createCloneButton;
    [SerializeField] private Button endCreationButton;

    private CloneManager _cloneManager;

    private bool _jumpLatched;
    private bool _interactLatched;
    private bool _carryLatched;

    private void Start()
    {
        _cloneManager = FindObjectOfType<CloneManager>();
        if (_cloneManager == null)
            Debug.LogWarning("[MobileInputUI] CloneManager를 찾을 수 없습니다.");

        SetCloneButtonState(isRecording: false);
    }

    public Vector2 MoveDirection => joystick != null ? joystick.Direction : Vector2.zero;

    /// <summary>
    /// 점프 버튼 Button 컴포넌트의 onClick에 인스펙터에서 연결.
    /// </summary>
    public void OnJumpPressed()
    {
        _jumpLatched = true;
    }

    /// <summary>
    /// 인터랙트 버튼 Button 컴포넌트의 onClick에 인스펙터에서 연결.
    /// </summary>
    public void OnInteractPressed()
    {
        _interactLatched = true;
    }

    /// <summary>
    /// 캐리 버튼 Button 컴포넌트의 onClick에 인스펙터에서 연결.
    /// </summary>
    public void OnCarryPressed()
    {
        _carryLatched = true;
    }

    public bool ConsumeJump()
    {
        if (!_jumpLatched) return false;
        _jumpLatched = false;
        return true;
    }

    public bool ConsumeInteract()
    {
        if (!_interactLatched) return false;
        _interactLatched = false;
        return true;
    }

    public bool ConsumeCarry()
    {
        if (!_carryLatched) return false;
        _carryLatched = false;
        return true;
    }

    public void OnCreateClonePressed()
    {
        if (_cloneManager == null) return;
        _cloneManager.OnCreateClone();
        SetCloneButtonState(isRecording: true);
    }

    public void OnEndCreationPressed()
    {
        if (_cloneManager == null) return;
        _cloneManager.OnEndCreation();
        SetCloneButtonState(isRecording: false);
    }

    private void SetCloneButtonState(bool isRecording)
    {
        if (createCloneButton != null) createCloneButton.gameObject.SetActive(!isRecording);
        if (endCreationButton != null)  endCreationButton.gameObject.SetActive(isRecording);
    }
}
