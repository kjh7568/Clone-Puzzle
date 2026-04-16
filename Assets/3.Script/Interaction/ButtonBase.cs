using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// IInteractable을 구현하는 버튼 계열 추상 베이스 클래스.
/// 현재 올라서 있는 Actor 목록을 관리하고, 활성/비활성 전환 시
/// 추상 메서드와 UnityEvent를 통해 하위 클래스·인스펙터에 통보한다.
/// </summary>
public abstract class ButtonBase : MonoBehaviour, IInteractable
{
    [Header("Button Events")]
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    protected List<Actor> activeActors = new List<Actor>();

    // ── IInteractable ──────────────────────────────────────────────

    public virtual bool CanInteract(Actor actor) => true;

    public virtual void OnInteractEnter(Actor actor)
    {
        if (!CanInteract(actor)) return;
        if (activeActors.Contains(actor)) return;

        bool wasActive = IsActive;
        activeActors.Add(actor);

        if (!wasActive && IsActive)
        {
            OnActivated();
            OnActivate.Invoke();
        }
    }

    public virtual void OnInteractExit(Actor actor)
    {
        if (!activeActors.Remove(actor)) return;

        if (!IsActive)
        {
            OnDeactivated();
            OnDeactivate.Invoke();
        }
    }

    // ── 상태 ────────────────────────────────────────────────────────

    /// <summary>하나 이상의 Actor가 버튼을 누르고 있으면 true.</summary>
    public virtual bool IsActive => activeActors.Count > 0;

    // ── 하위 클래스 구현 대상 ────────────────────────────────────────

    /// <summary>버튼이 비활성 → 활성으로 전환될 때 호출.</summary>
    protected abstract void OnActivated();

    /// <summary>버튼이 활성 → 비활성으로 전환될 때 호출.</summary>
    protected abstract void OnDeactivated();
}
