using UnityEngine;

/// <summary>
/// 누르고 있는 동안만 활성 상태를 유지하는 버튼.
/// Actor가 1명 이상 올라서 있으면 활성(pressed), 모두 떠나면 비활성(released).
/// </summary>
public class HoldButton : ButtonBase
{
    [Header("Visual")]
    [SerializeField] private GameObject releasedSprite;
    [SerializeField] private GameObject pressedSprite;

    private void Awake()
    {
        SetVisual(false);
    }

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

    protected override void OnActivated()   => SetVisual(true);
    protected override void OnDeactivated() => SetVisual(false);

    private void SetVisual(bool pressed)
    {
        if (releasedSprite != null) releasedSprite.SetActive(!pressed);
        if (pressedSprite  != null) pressedSprite.SetActive(pressed);
    }
}
