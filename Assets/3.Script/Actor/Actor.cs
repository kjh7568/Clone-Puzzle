using UnityEngine;

/// <summary>
/// 플레이어/클론 공통 베이스 클래스.
/// IInputProvider에서 입력을 받아 Rigidbody2D로 이동·점프를 처리한다.
/// 인터랙션 로직에서 플레이어/클론 분기 없이 동작하도록 설계.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Actor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Carry")]
    [SerializeField] private Transform carryAnchor;

    public Transform CarryAnchor => carryAnchor;
    public ICarryable CarriedObject { get; protected set; }
    public Vector2 FacingDirection { get; private set; } = Vector2.right;

    protected IInputProvider InputProvider;

    private Rigidbody2D _rb;
    private bool _isGrounded;

    // ── 수명 ────────────────────────────────────────────────────────────────
    private float _lifespan;
    private float _maxLifespan;
    private float _moveCostPerSecond;
    private float _jumpCost;
    private float _carryCost;
    private bool _isDepleted;
    private bool _lifespanInitialized;

    public float LifespanRatio => _maxLifespan > 0f ? _lifespan / _maxLifespan : 1f;
    public float CarryCost => _carryCost;

    protected Rigidbody2D Rb => _rb;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void InitLifespan(StageData data)
    {
        _maxLifespan = data.maxLifespan;
        _lifespan = data.maxLifespan;
        _moveCostPerSecond = data.moveCostPerSecond;
        _jumpCost = data.jumpCost;
        _carryCost = data.carryCost;
        _isDepleted = false;
        _lifespanInitialized = true;
    }

    public void ClearLifespan()
    {
        _lifespanInitialized = false;
        _isDepleted = false;
    }

    public void DeductLifespan(float amount)
    {
        if (!_lifespanInitialized || _isDepleted) return;
        _lifespan = Mathf.Max(0f, _lifespan - amount);
        if (_lifespan <= 0f)
        {
            _isDepleted = true;
            _rb.linearVelocity = Vector2.zero;
            Debug.Log($"[Actor] {gameObject.name} 수명 소진 — 동작 중단");
        }
    }

    protected virtual void FixedUpdate()
    {
        if (InputProvider == null || _isDepleted) return;

        CheckGround();
        ApplyMove(InputProvider.MoveDirection);
        ApplyJump(InputProvider.ConsumeJump());
    }

    public void SetInputProvider(IInputProvider provider)
    {
        InputProvider = provider;
    }

    /// <summary>CarrySystem에서 CarriedObject를 갱신할 때 사용.</summary>
    public void SetCarriedObject(ICarryable carryable)
    {
        CarriedObject = carryable;
    }

    /// <summary>InputProvider에서 캐리 입력을 소비해 반환. InputProvider가 없으면 false.</summary>
    public bool ConsumeCarry()
    {
        return InputProvider?.ConsumeCarry() ?? false;
    }

    private void CheckGround()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ApplyMove(Vector2 direction)
    {
        if (direction.x != 0f)
        {
            FacingDirection = direction.x > 0f ? Vector2.right : Vector2.left;
            DeductLifespan(_moveCostPerSecond * Time.fixedDeltaTime);
        }

        _rb.linearVelocity = new Vector2(direction.x * moveSpeed, _rb.linearVelocity.y);
    }

    private void ApplyJump(bool jumpConsumed)
    {
        if (jumpConsumed && _isGrounded)
        {
            DeductLifespan(_jumpCost);
            float force = CarriedObject != null ? jumpForce * 0.5f : jumpForce;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, force);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
