using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Box : MonoBehaviour, ICarryable, IResettable
{
    [Header("Arc Pickup")]
    [SerializeField] private float arcHeight   = 1.5f;
    [SerializeField] private float arcDuration = 1.5f;

    private Rigidbody2D  _rb;
    private BoxCollider2D _col;
    private bool          _isCarried;
    private Actor         _carrier;
    private Coroutine     _arcCoroutine;
    private int           _carryableLayer;
    private int           _blockingCount;

    public float ArcHeight => arcHeight;

    // ── ICarryable ──────────────────────────────────────────────

    public bool IsCarried => _isCarried;

    public Transform CarryAnchor => _carrier != null ? _carrier.CarryAnchor : null;

    public void OnPickedUp(Actor carrier)
    {
        if (_isCarried) return;

        _carrier   = carrier;
        _isCarried = true;

        _rb.bodyType       = RigidbodyType2D.Kinematic;
        _rb.linearVelocity = Vector2.zero;
        _rb.constraints    = RigidbodyConstraints2D.FreezeRotation;

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

    // ── 스택 밀기 차단 ───────────────────────────────────────────

    private void FixedUpdate()
    {
        if (_isCarried) return;

        int count = CountBlockingNeighbors();
        if (count == _blockingCount) return;

        _blockingCount = count;
        UpdateConstraints();
    }

    private int CountBlockingNeighbors()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            new Vector2(_col.bounds.size.x + 0.2f, _col.bounds.size.y + 0.2f),
            0f,
            1 << _carryableLayer);

        int count = 0;
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            float dy = hit.transform.position.y - transform.position.y;
            float dx = Mathf.Abs(hit.transform.position.x - transform.position.x);

            bool isAbove = dy > 0.3f;
            bool isSide  = dx > 0.3f && Mathf.Abs(dy) < 0.5f;

            if (isAbove || isSide) count++;
        }
        return count;
    }

    private void UpdateConstraints()
    {
        _rb.constraints = _blockingCount > 0
            ? RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX
            : RigidbodyConstraints2D.FreezeRotation;
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

    public void DrawArcGizmo(Vector3 anchorPos)
    {
        const int segments = 20;
        Vector3 p0 = transform.position;
        Vector3 p2 = anchorPos;
        Vector3 p1 = (p0 + p2) * 0.5f + Vector3.up * arcHeight;

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

        transform.position = _initialPosition;
        _rb.bodyType       = RigidbodyType2D.Dynamic;
        _rb.linearVelocity = Vector2.zero;
        _col.enabled       = true;
        _blockingCount     = 0;
        UpdateConstraints();
    }

    // ── Unity ───────────────────────────────────────────────────

    private void Awake()
    {
        _rb             = GetComponent<Rigidbody2D>();
        _col            = GetComponent<BoxCollider2D>();
        _initialPosition = transform.position;
        _carryableLayer = LayerMask.NameToLayer("Carryable");
    }
}
