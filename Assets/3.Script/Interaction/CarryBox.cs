using UnityEngine;

/// <summary>
/// Actor가 들고 다닐 수 있는 상자.
/// ICarryable을 구현하며 OnPickedUp/OnPutDown에서 물리·콜라이더 상태를 전환한다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class CarryBox : MonoBehaviour, ICarryable
{
    private Rigidbody2D _rb;
    private BoxCollider2D _col;
    private bool _isCarried;
    private Actor _carrier;

    // ── ICarryable ──────────────────────────────────────────────

    /// <inheritdoc/>
    public bool IsCarried => _isCarried;

    /// <summary>
    /// 현재 carrier의 CarryAnchor를 반환한다.
    /// 들려있지 않은 경우 null.
    /// </summary>
    public Transform CarryAnchor => _carrier != null ? _carrier.CarryAnchor : null;

    /// <summary>
    /// Actor가 상자를 들어올릴 때 호출.
    /// 물리를 비활성화하고 carrier의 CarryAnchor에 자식으로 붙는다.
    /// </summary>
    public void OnPickedUp(Actor carrier)
    {
        if (_isCarried) return;

        _carrier = carrier;
        _isCarried = true;

        _rb.isKinematic = true;
        _col.enabled = false;

        transform.SetParent(carrier.CarryAnchor);
        transform.position = carrier.CarryAnchor.position;
    }

    /// <summary>
    /// Actor가 상자를 내려놓을 때 호출.
    /// 지정된 위치로 이동 후 물리·콜라이더를 복원하고 부모에서 분리한다.
    /// </summary>
    public void OnPutDown(Vector2 position)
    {
        if (!_isCarried) return;

        transform.SetParent(null);
        transform.position = position;

        _rb.isKinematic = false;
        _col.enabled = true;

        _carrier = null;
        _isCarried = false;
    }

    // ── Unity ───────────────────────────────────────────────────

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<BoxCollider2D>();
    }
}
