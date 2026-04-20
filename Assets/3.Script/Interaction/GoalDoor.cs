using UnityEngine;

/// <summary>
/// Actor가 트리거 영역에 진입하면 StageManager.ClearStage()를 호출한다.
/// IsTrigger가 활성화된 Collider2D 필요.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class GoalDoor : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Actor>(out _)) return;
        if (StageManager.Instance == null)
        {
            Debug.LogWarning("[GoalDoor] StageManager 인스턴스 없음");
            return;
        }
        AudioManager.Instance?.PlaySFX(SfxType.StageCleared);
        StageManager.Instance.ClearStage();
    }
}
