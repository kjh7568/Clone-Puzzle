using UnityEngine;

/// <summary>
/// 누르고 있는 동안만 활성 상태를 유지하는 버튼.
/// Actor가 1명 이상 올라서 있으면 활성(pressed), 모두 떠나면 비활성(released).
/// SpriteRenderer 색상으로 눌림/뗌 상태를 시각적으로 구분한다.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class HoldButton : ButtonBase
{
    [Header("Visual")]
    [SerializeField] private Color pressedColor  = Color.green;
    [SerializeField] private Color releasedColor = Color.white;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = releasedColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Actor>(out var actor))
        {
            OnInteractEnter(actor);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Actor>(out var actor))
            OnInteractExit(actor);
    }

    protected override void OnActivated()
    {
        _spriteRenderer.color = pressedColor;
    }

    protected override void OnDeactivated()
    {
        _spriteRenderer.color = releasedColor;
    }
}
