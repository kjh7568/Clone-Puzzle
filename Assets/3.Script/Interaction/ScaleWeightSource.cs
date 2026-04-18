using UnityEngine;

/// <summary>
/// 저울에 무게를 제공하는 오브젝트에 붙이는 컴포넌트.
/// 매 FixedUpdate마다 아래를 체크해 ScalePlatform만 있으면 등록, 다른 지면도 있으면 해제.
/// Actor는 CarriedObject 여부로 weight 2 반환, CarryBox는 들렸을 때 해제.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ScaleWeightSource : MonoBehaviour
{
    [SerializeField] private float checkDistance = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Collider2D _collider;
    private Actor _actor;
    private ICarryable _carryable;
    private ScalePlatform _currentPlatform;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _actor = GetComponent<Actor>();
        _carryable = GetComponent<ICarryable>();
    }

    private void FixedUpdate()
    {
        if (_carryable != null && _carryable.IsCarried)
        {
            Unregister();
            return;
        }

        ScalePlatform found = FindScalePlatformBelow();

        if (found != null)
        {
            if (found != _currentPlatform)
            {
                Unregister();
                _currentPlatform = found;
            }
            // 등록 또는 weight 갱신 (캐리 상태 변화 반영)
            found.Register(this, GetWeight());
        }
        else
        {
            Unregister();
        }
    }

    private ScalePlatform FindScalePlatformBelow()
    {
        Bounds b = _collider.bounds;
        Vector2 center = new Vector2(b.center.x, b.min.y - checkDistance * 0.5f);
        Vector2 size   = new Vector2(b.size.x * 0.8f, checkDistance);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, groundLayer);

        ScalePlatform scalePlatform = null;
        bool hasOtherGround = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            if (hit.isTrigger) continue;

            ScalePlatform sp = hit.GetComponentInParent<ScalePlatform>();
            if (sp != null)
                scalePlatform = sp;
            else
                hasOtherGround = true;
        }

        return (scalePlatform != null && !hasOtherGround) ? scalePlatform : null;
    }

    private int GetWeight()
    {
        if (_actor != null)
            return 1 + (_actor.CarriedObject != null ? 1 : 0);
        return 1;
    }

    private void Unregister()
    {
        if (_currentPlatform == null) return;
        _currentPlatform.Unregister(this);
        _currentPlatform = null;
    }

    private void OnDisable() => Unregister();
    private void OnDestroy() => Unregister();

    private void OnDrawGizmosSelected()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null) return;

        Bounds b = col.bounds;
        Vector3 center = new Vector3(b.center.x, b.min.y - checkDistance * 0.5f, 0f);
        Vector3 size   = new Vector3(b.size.x * 0.8f, checkDistance, 0f);

        Gizmos.color = _currentPlatform != null ? Color.green : Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}
