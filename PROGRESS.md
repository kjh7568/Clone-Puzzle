# Clone Puzzle — Progress

## 현재 단계
**S1** — 코어 게임플레이 구현 중

---

## 구현 완료 파일 (32개)

### Actor
- [x] `Actor.cs` — 플레이어/클론 공통 베이스 (이동, 점프, 수명)
- [x] `PlayerController.cs` — 플레이어 입력 처리
- [x] `CloneManager.cs` — 클론 생성/재생/리셋 관리
- [x] `ClonePlayback.cs` — 녹화 입력 재생 클론
- [x] `CarrySystem.cs` — 들기/내려놓기 처리, 근접 상자 감지(CanCarry)
- [x] `PlayerAnimatorController.cs` — 플레이어 애니메이션 제어

### Recording
- [x] `InputRecorder.cs` — FixedUpdate 단위 입력 녹화
- [x] `FrameInput.cs` — 프레임 입력 데이터 구조체

### Interaction
- [x] `ButtonBase.cs` — 버튼 인터랙션 베이스
- [x] `HoldButton.cs` — 누르고 있어야 활성화되는 버튼
- [x] `ToggleButton.cs` — 토글 버튼
- [x] `Door.cs` — 문 열림/닫힘
- [x] `GoalDoor.cs` — 클리어 트리거 (SFX 연결됨)
- [x] `CarryBox.cs` — 들 수 있는 상자
- [x] `ScaleController.cs` — 저울 제어
- [x] `ScalePlatform.cs` — 무게 기반 플랫폼
- [x] `ScaleWeightSource.cs` — 무게 소스

### Interface
- [x] `IInputProvider.cs`
- [x] `ICarryable.cs`
- [x] `IInteractable.cs`
- [x] `IRecordable.cs`
- [x] `IResettable.cs`

### Stage
- [x] `StageData.cs` — 스테이지 설정 ScriptableObject
- [x] `StageManager.cs` — 스테이지 클리어 및 씬 전환

### UI
- [x] `UIManager.cs` — UI 전반, 모바일 입력, SFX 연결, 캐리 버튼 활성화
- [x] `VirtualJoystick.cs` — 온스크린 조이스틱
- [x] `LifespanBar.cs` — 수명 바 UI

### Audio
- [x] `AudioManager.cs` — BGM/SFX 싱글톤 매니저 (DontDestroyOnLoad)
- [x] `AudioData.cs` — 오디오 클립 ScriptableObject (명명 필드)
- [x] `SoundType.cs` — BgmType / SfxType 열거형
- [x] `SceneBGM.cs` — 씬별 BGM 자동 재생 컴포넌트

### Test
- [x] `TestCloneManager.cs` — CloneManager 단위 테스트

---

## 알려진 버그
없음

---

## 보류 항목
없음
