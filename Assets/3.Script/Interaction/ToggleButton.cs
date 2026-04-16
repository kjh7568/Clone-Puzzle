using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OnInteractEnter 호출마다 활성/비활성을 토글하는 버튼.
/// Actor가 트리거 범위에 진입한 뒤 인터랙트 키를 누를 때 토글된다.
/// 동일 Actor의 연속 토글을 cooldown으로 방지한다.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ToggleButton : ButtonBase
{
    [Header("Toggle Settings")]
    [SerializeField] private float cooldown = 0.5f;

    [Header("Visual")]
    [SerializeField] private Color activeColor   = Color.green;
    [SerializeField] private Color inactiveColor = Color.gray;

    private bool _toggleState;
    private SpriteRenderer _spriteRenderer;
    private readonly Dictionary<Actor, float> _cooldownTracker = new Dictionary<Actor, float>();

    // ── 상태 ────────────────────────────────────────────────────────

    /// <summary>현재 토글 상태. true = 활성.</summary>
    public override bool IsActive => _toggleState;

    // ── Unity 생명주기 ───────────────────────────────────────────────

    protected virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    // ── IInteractable ──────────────────────────────────────────────

    /// <summary>해당 Actor의 cooldown이 지났을 때만 인터랙션 허용.</summary>
    public override bool CanInteract(Actor actor)
    {
        return !_cooldownTracker.TryGetValue(actor, out float lastTime)
            || Time.time - lastTime >= cooldown;
    }

    /// <summary>호출될 때마다 토글. cooldown 미경과 Actor는 무시.</summary>
    public override void OnInteractEnter(Actor actor)
    {
        if (!CanInteract(actor)) return;

        _cooldownTracker[actor] = Time.time;
        _toggleState = !_toggleState;

        if (_toggleState)
        {
            OnActivated();
            OnActivate.Invoke();
        }
        else
        {
            OnDeactivated();
            OnDeactivate.Invoke();
        }
    }

    /// <summary>토글 버튼은 Exit 이벤트를 무시한다.</summary>
    public override void OnInteractExit(Actor actor) { }

    // ── 트리거 감지 ──────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Actor>(out var actor))
            OnInteractEnter(actor);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Actor>(out var actor))
            OnInteractExit(actor);
    }

    // ── ButtonBase 추상 구현 ─────────────────────────────────────────

    protected override void OnActivated()   => UpdateVisual();
    protected override void OnDeactivated() => UpdateVisual();

    // ── 내부 ────────────────────────────────────────────────────────

    private void UpdateVisual()
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.color = _toggleState ? activeColor : inactiveColor;
    }
}
