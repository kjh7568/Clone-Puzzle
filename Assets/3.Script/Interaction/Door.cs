using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 버튼 등 외부 오브젝트가 Open()/Close()를 호출해 제어하는 문.
/// IInteractable을 구현하지 않으며, ButtonBase와의 연결은 Inspector UnityEvent로만 처리한다.
/// 열릴 때 DOTween으로 Y축 이동, 닫힐 때 원위치로 복귀.
/// </summary>
public class Door : MonoBehaviour, IResettable
{
    [Header("Animation")]
    [SerializeField] private float openOffsetY = 2f;
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private Ease ease = Ease.InOutQuad;
    [SerializeField] private float closeDelay = 1f;

    [Header("Door Events")]
    public UnityEvent OnOpened;
    public UnityEvent OnClosed;

    private Collider2D _collider;
    private Vector3 _closedPosition;
    private bool _isOpen;
    private Coroutine _closeCoroutine;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _closedPosition = transform.position;
    }

    // ── Public API ──────────────────────────────────────────────────

    /// <summary>문을 연다. Y축으로 openOffsetY만큼 이동, 콜라이더 비활성화, OnOpened 발행.</summary>
    public void Open()
    {
        if (_isOpen) return;
        if (_closeCoroutine != null) { StopCoroutine(_closeCoroutine); _closeCoroutine = null; }
        _isOpen = true;

        if (_collider != null) _collider.enabled = false;

        Vector3 target = _closedPosition + new Vector3(0f, openOffsetY, 0f);
        transform.DOMove(target, duration).SetEase(ease)
            .OnComplete(() => OnOpened.Invoke());
    }

    /// <summary>closeDelay초 후 문을 닫는다. 버튼 해제 시 사용.</summary>
    public void CloseDelayed()
    {
        if (!_isOpen) return;
        if (_closeCoroutine != null) StopCoroutine(_closeCoroutine);
        _closeCoroutine = StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);
        _closeCoroutine = null;
        Close();
    }

    /// <summary>문을 닫는다. 원위치로 복귀, 콜라이더 활성화, OnClosed 발행.</summary>
    public void Close()
    {
        if (!_isOpen) return;
        _isOpen = false;

        transform.DOMove(_closedPosition, duration).SetEase(ease)
            .OnComplete(() =>
            {
                if (_collider != null) _collider.enabled = true;
                OnClosed.Invoke();
            });
    }

    /// <summary>현재 열려 있는지 반환.</summary>
    public bool IsOpen => _isOpen;

    // ── IResettable ──────────────────────────────────────────────

    public void SaveInitialState() { } // _closedPosition을 Awake에서 이미 저장

    public void ResetState()
    {
        if (_closeCoroutine != null) { StopCoroutine(_closeCoroutine); _closeCoroutine = null; }
        DOTween.Kill(transform);
        _isOpen = false;
        transform.position = _closedPosition;
        if (_collider != null) _collider.enabled = true;
    }
}
