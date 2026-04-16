using UnityEngine;

/// <summary>
/// Actor가 들고 다닐 수 있는 오브젝트의 인터페이스.
/// 상자·아이템 등 캐리 가능한 오브젝트가 구현한다.
/// </summary>
public interface ICarryable
{
    /// <summary>Actor가 오브젝트를 들어올릴 때 호출.</summary>
    void OnPickedUp(Actor carrier);

    /// <summary>Actor가 오브젝트를 내려놓을 때 호출. position은 내려놓을 월드 좌표.</summary>
    void OnPutDown(Vector2 position);

    /// <summary>현재 Actor에게 들려있는 상태인지 여부.</summary>
    bool IsCarried { get; }

    /// <summary>들고 있을 때 오브젝트가 붙을 위치 (보통 Actor 머리 위).</summary>
    Transform CarryAnchor { get; }
}
