using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    public UnityEvent OnStageCleared;

    private bool _isCleared;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ClearStage()
    {
        if (_isCleared) return;
        _isCleared = true;

        int n = StageProgressManager.ParseStageNumber(SceneManager.GetActiveScene().name);
        if (n > 0) StageProgressManager.Instance.MarkCleared(n);

        Debug.Log("[StageManager] 스테이지 클리어!");
        OnStageCleared.Invoke();
    }
}
