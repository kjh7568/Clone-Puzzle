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
    [SerializeField] private float boxHalfWidth = 0.5f;   // 상자 콜라이더 반폭 (1x1 기준 0.5)
    [SerializeField] private float minPutDownSpace = 0.1f; // 벽과 상자 사이 최소 여유 공간
    [SerializeField] private LayerMask obstacleLayer;

    private Actor _actor;

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
    }

    // ── 내려놓기 ────────────────────────────────────────────────────────────

    private void PutDown()
    {
        Vector2 origin    = transform.position;
        Vector2 direction = _actor.FacingDirection; // left or right only

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, putDownDistance, obstacleLayer);

        float placeDistance;
        if (hit.collider == null)
        {
            // 앞 1칸까지 장애물 없음 → 목표 거리에 배치
            placeDistance = putDownDistance;
        }
        else
        {
            // 벽 직전에 상자 절반 + 여유 공간만큼 안쪽에 배치
            placeDistance = hit.distance - boxHalfWidth - minPutDownSpace;
            if (placeDistance <= 0f) return; // 공간 부족 → 내려놓지 않음
        }

        Vector2 targetPos = origin + direction * placeDistance;
        _actor.CarriedObject.OnPutDown(targetPos);
        _actor.SetCarriedObject(null);
    }

    // ── Gizmos ──────────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);

        Vector2 dir      = Application.isPlaying ? _actor.FacingDirection : Vector2.right;
        Vector2 frontPos = (Vector2)transform.position + dir * putDownDistance;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(frontPos, 0.1f);
        Gizmos.DrawLine(transform.position, frontPos);
    }
}
