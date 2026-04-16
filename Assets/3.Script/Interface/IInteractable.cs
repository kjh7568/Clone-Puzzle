/// <summary>
/// Actor가 인터랙션할 수 있는 오브젝트의 인터페이스.
/// 버튼·레버·문 등 모든 상호작용 가능 오브젝트가 구현한다.
/// </summary>
public interface IInteractable
{
    /// <summary>인터랙션 시작 — 버튼을 누르거나 범위에 진입했을 때.</summary>
    void OnInteractEnter(Actor actor);

    /// <summary>인터랙션 종료 — 버튼을 떼거나 범위를 벗어났을 때.</summary>
    void OnInteractExit(Actor actor);

    /// <summary>현재 actor가 인터랙션 가능한 상태인지 반환.</summary>
    bool CanInteract(Actor actor);
}
