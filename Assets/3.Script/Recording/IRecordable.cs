using System.Collections.Generic;

/// <summary>
/// 녹화 가능한 오브젝트(주로 PlayerController)가 구현하는 인터페이스.
/// InputRecorder가 이 인터페이스를 통해 녹화를 제어하고 데이터를 수집한다.
/// </summary>
public interface IRecordable
{
    /// <summary>녹화 시작. 타임스탬프 초기화 및 버퍼 클리어.</summary>
    void StartRecording();

    /// <summary>녹화 중단.</summary>
    void StopRecording();

    /// <summary>녹화된 FrameInput 목록 반환. ClonePlayback에서 소비.</summary>
    List<FrameInput> GetRecordedData();
}
