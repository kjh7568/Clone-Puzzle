using UnityEngine;

/// <summary>
/// 4개 애니메이션 상태(Idle, Walk, Jump, Pickup)를 구동하는 컨트롤러.
/// IsGrounded는 Actor에서, 픽업 이벤트는 CarrySystem에서 수신한다.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimatorController : MonoBehaviour
{
    private static readonly int ParamSpeed      = Animator.StringToHash("Speed");
    private static readonly int ParamIsGrounded = Animator.StringToHash("IsGrounded");
    private static readonly int ParamPickup     = Animator.StringToHash("Pickup");

    private Animator        _animator;
    private Rigidbody2D     _rb;
    private Actor           _actor;
    private CarrySystem     _carrySystem;
    private SpriteRenderer  _spriteRenderer;

    private void Awake()
    {
        _animator       = GetComponent<Animator>();
        _rb             = GetComponent<Rigidbody2D>();
        _actor          = GetComponent<Actor>();
        _carrySystem    = GetComponent<CarrySystem>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (_carrySystem != null)
            _carrySystem.OnPickedUp += TriggerPickup;
    }

    private void OnDisable()
    {
        if (_carrySystem != null)
            _carrySystem.OnPickedUp -= TriggerPickup;
    }

    private void Update()
    {
        float vx = _rb.linearVelocity.x;
        _animator.SetFloat(ParamSpeed, Mathf.Abs(vx));
        _animator.SetBool(ParamIsGrounded, _actor.IsGrounded);

        if (vx < -0.01f)       _spriteRenderer.flipX = true;
        else if (vx > 0.01f)   _spriteRenderer.flipX = false;
    }

    public void TriggerPickup()
    {
        _animator.SetTrigger(ParamPickup);
    }
}
