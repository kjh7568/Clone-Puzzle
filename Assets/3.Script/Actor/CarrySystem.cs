using System;
using UnityEngine;

/// <summary>
/// Actor에 붙는 캐리(들기/내려놓기) 처리 컴포넌트.
/// FrameInput.carryPressed를 감지해 주변 ICarryable을 들거나 내려놓는다.
/// 플레이어/클론 모두 Actor 기반으로 동일하게 동작한다.
/// </summary>
[RequireComponent(typeof(Actor))]
public class CarrySystem : MonoBehaviour
{
    [Header("Pickup")]
    [SerializeField] private float pickupRadius = 1.5f;
    [SerializeField] private LayerMask carryableLayer;

    [Header("Put Down")]
    [SerializeField] private float putDownDistance = 1f;
    [SerializeField] private float boxHalfWidth = 0.5f;
    [SerializeField] private float minPutDownSpace = 0.1f;
    [SerializeField] private LayerMask obstacleLayer;

    private Actor _actor;

    public event Action OnPickedUp;
    public bool CanCarry => _actor.CarriedObject != null || HasNearbyCarryable();

    private void Awake()
    {
        _actor = GetComponent<Actor>();
    }

    private void FixedUpdate()
    {
        if (!_actor.ConsumeCarry()) return;

        if (_actor.CarriedObject == null)
            TryPickUp();
        else
            PutDown();
    }

    // ── 들기 ────────────────────────────────────────────────────────────────

    private bool HasNearbyCarryable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius, carryableLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<ICarryable>(out var c) && !c.IsCarried)
                return true;
        }
        return false;
    }

    private void TryPickUp()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRadius, carryableLayer);

        ICarryable nearest = null;
        float minDist = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            ICarryable carryable = hit.GetComponent<ICarryable>();
            if (carryable == null || carryable.IsCarried) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = carryable;
            }
        }

        if (nearest == null) return;

        nearest.OnPickedUp(_actor);
        _actor.SetCarriedObject(nearest);
        _actor.DeductLifespan(_actor.CarryCost);
        OnPickedUp?.Invoke();
    }

    // ── 내려놓기 ────────────────────────────────────────────────────────────

    private void PutDown()
    {
        Vector2 origin    = transform.position;
        Vector2 direction = _actor.FacingDirection;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, putDownDistance, obstacleLayer);

        float placeDistance;
        if (hit.collider == null)
        {
            placeDistance = putDownDistance;
        }
        else
        {
            placeDistance = hit.distance - boxHalfWidth - minPutDownSpace;
            if (placeDistance <= 0f) return;
        }

        Vector2 targetPos = origin + direction * placeDistance;
        _actor.CarriedObject.OnPutDown(targetPos);
        _actor.SetCarriedObject(null);
    }

    // ── Gizmos ──────────────────────────────────────────────────────────────

    private void OnDrawGizmos()
    {
        // 픽업 반경
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);

        // 내려놓기 예상 위치
        Vector2 dir      = Application.isPlaying && _actor != null
                           ? _actor.FacingDirection
                           : Vector2.right;
        Vector2 frontPos = (Vector2)transform.position + dir * putDownDistance;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(frontPos, 0.1f);
        Gizmos.DrawLine(transform.position, frontPos);

        // 픽업 범위 내 CarryBox 포물선 미리보기
        Actor selfActor = _actor != null ? _actor : GetComponent<Actor>();
        if (selfActor == null || selfActor.CarryAnchor == null) return;

        CarryBox[] allBoxes = UnityEngine.Object.FindObjectsByType<CarryBox>(FindObjectsSortMode.None);
        Vector3 anchorPos   = selfActor.CarryAnchor.position;

        foreach (CarryBox box in allBoxes)
        {
            if (box == null) continue;
            if (Vector3.Distance(transform.position, box.transform.position) <= pickupRadius)
                box.DrawArcGizmo(anchorPos);
        }
    }
}
