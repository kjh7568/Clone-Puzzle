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

    protected IInputProvider InputProvider;

    private Rigidbody2D _rb;
    private bool _isGrounded;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (InputProvider == null) return;

        CheckGround();
        ApplyMove(InputProvider.MoveDirection);
        ApplyJump(InputProvider.ConsumeJump());
    }

    public void SetInputProvider(IInputProvider provider)
    {
        InputProvider = provider;
    }

    private void CheckGround()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ApplyMove(Vector2 direction)
    {
        _rb.linearVelocity = new Vector2(direction.x * moveSpeed, _rb.linearVelocity.y);
    }

    private void ApplyJump(bool jumpConsumed)
    {
        if (jumpConsumed && _isGrounded)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
