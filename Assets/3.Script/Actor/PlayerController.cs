using UnityEngine;

/// <summary>
/// 플레이어 캐릭터 컨트롤러.
/// MobileInputUI의 입력을 매 프레임 InputRecorder.SetInput()으로 주입하고,
/// Actor에는 InputRecorder를 IInputProvider로 연결한다.
/// InputRecorder가 입력을 기록하면서 동시에 Actor에 전달하는 중간 레이어 역할을 한다.
/// </summary>
public class PlayerController : Actor, IResettable
{
    [Header("Input")]
    [SerializeField] private MobileInputUI mobileInput;
    [SerializeField] private InputRecorder inputRecorder;

    private Vector3 _initialPosition;

    protected override void Awake()
    {
        base.Awake();
        SetInputProvider(inputRecorder);
        _initialPosition = transform.position;
    }

    // ── IResettable ──────────────────────────────────────────────────────

    public void SaveInitialState() => _initialPosition = transform.position;

    public void ResetState()
    {
        transform.position = _initialPosition;
        Rb.linearVelocity = Vector2.zero;
        if (CarriedObject != null) SetCarriedObject(null);
    }

    private void Update()
    {
        inputRecorder.SetInput(
            mobileInput.MoveDirection,
            mobileInput.ConsumeJump(),
            mobileInput.ConsumeInteract(),
            mobileInput.ConsumeCarry()
        );
    }
}
