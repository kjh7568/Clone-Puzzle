using UnityEngine;

/// <summary>
/// 플레이어 행동에 맞는 Animator 파라미터를 관리하는 컨트롤러.
/// 외부 플레이어 스크립트에서 메서드를 호출하거나,
/// Rigidbody2D 속도를 자동으로 읽어 상태를 갱신한다.
///
/// [Animator 파라미터 목록]
/// - Speed         (Float)   : 수평 이동 속도 (0=Idle, >0=Walk/Run)
/// - IsSprinting   (Bool)    : 달리기 여부
/// - IsCrouching   (Bool)    : 앉기 여부
/// - IsGrounded    (Bool)    : 착지 여부
/// - VerticalVel   (Float)   : 수직 속도 (양수=상승, 음수=낙하)
/// - Attack        (Trigger) : 공격 트리거
/// - Interact      (Trigger) : 상호작용 트리거
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimatorController : MonoBehaviour
{
    // ── Animator 파라미터 해시 (string 비교 대신 int 해시 사용) ──────────────
    private static readonly int ParamSpeed       = Animator.StringToHash("Speed");
    private static readonly int ParamIsSprinting = Animator.StringToHash("IsSprinting");
    private static readonly int ParamIsCrouching = Animator.StringToHash("IsCrouching");
    private static readonly int ParamIsGrounded  = Animator.StringToHash("IsGrounded");
    private static readonly int ParamVerticalVel = Animator.StringToHash("VerticalVel");
    private static readonly int ParamAttack      = Animator.StringToHash("Attack");
    private static readonly int ParamInteract    = Animator.StringToHash("Interact");

    // ── Ground 감지 설정 ─────────────────────────────────────────────────────
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float     groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    // ── 컴포넌트 참조 ────────────────────────────────────────────────────────
    private Animator     _animator;
    private Rigidbody2D  _rb;

    // ── 현재 상태 캐시 ───────────────────────────────────────────────────────
    private bool _isSprinting;
    private bool _isCrouching;

    // =========================================================================
    // Unity 생명주기
    // =========================================================================

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rb       = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateGrounded();
        UpdateMovement();
        UpdateVerticalVelocity();
    }

    // =========================================================================
    // 내부 자동 갱신
    // =========================================================================

    /// <summary>착지 여부를 OverlapCircle로 감지해 Animator에 전달.</summary>
    private void UpdateGrounded()
    {
        if (groundCheck == null) return;

        bool grounded = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);

        _animator.SetBool(ParamIsGrounded, grounded);
    }

    /// <summary>Rigidbody2D 수평 속도로 Speed 파라미터 갱신.</summary>
    private void UpdateMovement()
    {
        float speed = Mathf.Abs(_rb.linearVelocity.x);
        _animator.SetFloat(ParamSpeed, speed);
    }

    /// <summary>Rigidbody2D 수직 속도로 VerticalVel 파라미터 갱신.</summary>
    private void UpdateVerticalVelocity()
    {
        _animator.SetFloat(ParamVerticalVel, _rb.linearVelocity.y);
    }

    // =========================================================================
    // 외부 호출용 Public API
    // =========================================================================

    /// <summary>달리기 상태 설정. 플레이어 입력 스크립트에서 호출.</summary>
    public void SetSprinting(bool isSprinting)
    {
        _isSprinting = isSprinting;
        _animator.SetBool(ParamIsSprinting, _isSprinting);
    }

    /// <summary>앉기 상태 설정. 플레이어 입력 스크립트에서 호출.</summary>
    public void SetCrouching(bool isCrouching)
    {
        _isCrouching = isCrouching;
        _animator.SetBool(ParamIsCrouching, _isCrouching);
    }

    /// <summary>공격 트리거 발동. 플레이어 입력 스크립트에서 호출.</summary>
    public void TriggerAttack()
    {
        _animator.SetTrigger(ParamAttack);
    }

    /// <summary>상호작용 트리거 발동. 플레이어 입력 스크립트에서 호출.</summary>
    public void TriggerInteract()
    {
        _animator.SetTrigger(ParamInteract);
    }

    // =========================================================================
    // 디버그
    // =========================================================================

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
#endif
}
