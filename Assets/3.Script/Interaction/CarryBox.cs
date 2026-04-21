using System.Collections;
using UnityEngine;

/// <summary>
/// Actor가 들고 다닐 수 있는 상자.
/// ICarryable을 구현하며 OnPickedUp/OnPutDown에서 물리·콜라이더 상태를 전환한다.
/// 픽업 시 Quadratic Bezier 포물선으로 CarryAnchor까지 이동한다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class CarryBox : MonoBehaviour, ICarryable, IResettable
{
    [Header("Arc Pickup")]
    [SerializeField] private float arcHeight   = 1.5f;
    [SerializeField] private float arcDuration = 1.5f;

    private Rigidbody2D  _rb;
    private BoxCollider2D _col;
    private bool          _isCarried;
    private Actor         _carrier;
    private Coroutine     _arcCoroutine;

    public float ArcHeight => arcHeight;

    // ── ICarryable ──────────────────────────────────────────────

    public bool IsCarried => _isCarried;

    public Transform CarryAnchor => _carrier != null ? _carrier.CarryAnchor : null;

    public void OnPickedUp(Actor carrier)
    {
        if (_isCarried) return;

        _carrier   = carrier;
        _isCarried = true;

        _rb.bodyType        = RigidbodyType2D.Kinematic;
        _rb.linearVelocity  = Vector2.zero;

        Collider2D carrierCol = carrier.GetComponent<Collider2D>();
        if (carrierCol != null) Physics2D.IgnoreCollision(_col, carrierCol, true);

        _arcCoroutine = StartCoroutine(ArcMoveToAnchor(carrier));
    }

    public void OnPutDown(Vector2 position)
    {
        if (!_isCarried) return;

        if (_arcCoroutine != null)
        {
            StopCoroutine(_arcCoroutine);
            _arcCoroutine = null;
            transform.SetParent(null);
        }

        Collider2D carrierCol = _carrier.GetComponent<Collider2D>();
        if (carrierCol != null) Physics2D.IgnoreCollision(_col, carrierCol, false);

        transform.SetParent(null);
        transform.position = position;

        _rb.bodyType = RigidbodyType2D.Dynamic;

        _carrier   = null;
        _isCarried = false;
    }

    // ── Arc ─────────────────────────────────────────────────────

    private IEnumerator ArcMoveToAnchor(Actor carrier)
    {
        Vector3 p0      = transform.position;
        float   elapsed = 0f;

        while (elapsed < arcDuration)
        {
            elapsed += Time.deltaTime;
            float t  = Mathf.Clamp01(elapsed / arcDuration);

            Vector3 p2 = carrier.CarryAnchor.position;
            Vector3 p1 = (p0 + p2) * 0.5f + Vector3.up * arcHeight;
            float   u  = 1f - t;

            transform.position = u * u * p0 + 2f * u * t * p1 + t * t * p2;
            yield return null;
        }

        transform.position = carrier.CarryAnchor.position;
        transform.SetParent(carrier.CarryAnchor);
        _arcCoroutine = null;
    }

    // ── Gizmo ───────────────────────────────────────────────────

    /// <summary>CarrySystem.OnDrawGizmos에서 픽업 범위 내일 때 외부 호출.</summary>
    public void DrawArcGizmo(Vector3 anchorPos)
    {
        const int segments = 20;
        Vector3 p0   = transform.position;
        Vector3 p2   = anchorPos;
        Vector3 p1   = (p0 + p2) * 0.5f + Vector3.up * arcHeight;

        Gizmos.color = Color.green;
        Vector3 prev = p0;
        for (int i = 1; i <= segments; i++)
        {
            float   t  = i / (float)segments;
            float   u  = 1f - t;
            Vector3 pt = u * u * p0 + 2f * u * t * p1 + t * t * p2;
            Gizmos.DrawLine(prev, pt);
            prev = pt;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(p1, 0.08f);
    }

    // ── IResettable ──────────────────────────────────────────────

    private Vector3 _initialPosition;

    public void SaveInitialState() => _initialPosition = transform.position;

    public void ResetState()
    {
        if (_arcCoroutine != null)
        {
            StopCoroutine(_arcCoroutine);
            _arcCoroutine = null;
        }

        if (_isCarried)
        {
            Collider2D carrierCol = _carrier?.GetComponent<Collider2D>();
            if (carrierCol != null) Physics2D.IgnoreCollision(_col, carrierCol, false);

            transform.SetParent(null);
            _isCarried = false;
            _carrier   = null;
        }

        transform.position    = _initialPosition;
        _rb.bodyType          = RigidbodyType2D.Dynamic;
        _rb.linearVelocity    = Vector2.zero;
        _col.enabled          = true;
    }

    // ── Unity ───────────────────────────────────────────────────

    private void Awake()
    {
        _rb               = GetComponent<Rigidbody2D>();
        _col              = GetComponent<BoxCollider2D>();
        _initialPosition  = transform.position;
    }
}
