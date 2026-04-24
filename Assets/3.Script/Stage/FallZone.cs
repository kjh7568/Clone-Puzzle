using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 스테이지 하단 트리거. PlayerController가 진입하면 카메라 낙하 연출 후 씬을 재시작한다.
/// IsTrigger가 활성화된 Collider2D 필요.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FallZone : MonoBehaviour
{
    [SerializeField] float restartDelay = 1.5f;

    bool _triggered;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        if (!other.TryGetComponent<PlayerController>(out _)) return;

        _triggered = true;
        Camera.main.transform.SetParent(null);
        StartCoroutine(RestartAfterDelay());
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
