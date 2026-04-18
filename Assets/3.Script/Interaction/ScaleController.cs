using UnityEngine;

/// <summary>
/// 저울 루트 오브젝트. 좌우 ScalePlatform의 무게 차이를 계산해 플랫폼을 위아래로 이동시킨다.
/// 플랫폼은 Kinematic Rigidbody2D로 이동 → 위에 올라온 오브젝트가 함께 이동.
/// </summary>
public class ScaleController : MonoBehaviour
{
    [SerializeField] private ScalePlatform _leftPlatform;
    [SerializeField] private ScalePlatform _rightPlatform;

    [Header("Settings")]
    [SerializeField] private int _maxSteps = 3;
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody2D _leftRb;
    private Rigidbody2D _rightRb;
    private float _leftBaseY;
    private float _rightBaseY;

    private void Start()
    {
        _leftRb  = _leftPlatform.GetComponent<Rigidbody2D>();
        _rightRb = _rightPlatform.GetComponent<Rigidbody2D>();
        _leftBaseY  = _leftPlatform.transform.position.y;
        _rightBaseY = _rightPlatform.transform.position.y;
    }

    private void FixedUpdate()
    {
        int leftW  = _leftPlatform.TotalWeight;
        int rightW = _rightPlatform.TotalWeight;
        int diff   = Mathf.Clamp(leftW - rightW, -_maxSteps, _maxSteps);

        MoveToward(_leftRb,  _leftBaseY  - diff);
        MoveToward(_rightRb, _rightBaseY + diff);

        Debug.Log($"[Scale] Left:{leftW}  Right:{rightW}  Diff:{diff}");
    }

    private void MoveToward(Rigidbody2D rb, float targetY)
    {
        float newY = Mathf.MoveTowards(rb.position.y, targetY, _moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(new Vector2(rb.position.x, newY));
    }
}
