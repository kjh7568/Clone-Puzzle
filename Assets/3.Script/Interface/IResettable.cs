/// <summary>
/// 씬 리셋 시 초기 상태로 복원할 수 있는 오브젝트의 인터페이스.
/// CloneManager가 라운드 시작마다 ResetState()를 호출한다.
/// </summary>
public interface IResettable
{
    /// <summary>현재 상태를 초기 상태로 저장. Start() 또는 씬 배치 직후 호출.</summary>
    void SaveInitialState();

    /// <summary>저장된 초기 상태로 복원. 이벤트 발행 없이 내부 상태만 변경.</summary>
    void ResetState();
}
