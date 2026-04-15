using UnityEngine;

/// <summary>
/// 플레이어 캐릭터 컨트롤러.
/// MobileInputUI(IInputProvider)를 Actor에 주입해 이동·점프를 처리한다.
/// </summary>
public class PlayerController : Actor
{
    [Header("Input")]
    [SerializeField] private MobileInputUI mobileInput;

    protected override void Awake()
    {
        base.Awake();
        SetInputProvider(mobileInput);
    }
}
