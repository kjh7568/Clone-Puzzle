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

        Debug.Log("[StageManager] 스테이지 클리어!");
        OnStageCleared.Invoke();

        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            Debug.Log("[StageManager] 마지막 스테이지 클리어!");
    }
}
