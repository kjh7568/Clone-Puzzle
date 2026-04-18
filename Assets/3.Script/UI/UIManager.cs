using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// UI 전반 및 모바일 입력을 담당하는 매니저.
/// IInputProvider를 구현해 플레이어 입력을 제공하고,
/// 일시정지·클론·씬 전환 등 UI 기능을 처리한다.
/// </summary>
public class UIManager : MonoBehaviour, IInputProvider
{
    private const string StageSelectSceneName = "StageSelect";

    [Header("Input")]
    [SerializeField] private VirtualJoystick joystick;

    [Header("Clone Buttons")]
    [SerializeField] private Button createCloneButton;
    [SerializeField] private Button endCreationButton;

    [Header("Stage Text")]
    [SerializeField] private TMP_Text stageText;

    private CloneManager _cloneManager;

    private bool _jumpLatched;
    private bool _interactLatched;
    private bool _carryLatched;

    private void Start()
    {
        _cloneManager = FindFirstObjectByType<CloneManager>();
        if (_cloneManager == null)
            Debug.LogWarning("[UIManager] CloneManager를 찾을 수 없습니다.");

        SetCloneButtonState(isRecording: false);

        if (stageText != null)
            stageText.text = "현재 스테이지";
    }

    // ── IInputProvider ────────────────────────────────────────────────────

    public Vector2 MoveDirection => joystick != null ? joystick.Direction : Vector2.zero;

    public void OnJumpPressed()    => _jumpLatched    = true;
    public void OnInteractPressed() => _interactLatched = true;
    public void OnCarryPressed()   => _carryLatched   = true;

    public bool ConsumeJump()
    {
        if (!_jumpLatched) return false;
        _jumpLatched = false;
        return true;
    }

    public bool ConsumeInteract()
    {
        if (!_interactLatched) return false;
        _interactLatched = false;
        return true;
    }

    public bool ConsumeCarry()
    {
        if (!_carryLatched) return false;
        _carryLatched = false;
        return true;
    }

    // ── 클론 버튼 ─────────────────────────────────────────────────────────

    public void OnCreateClonePressed()
    {
        if (_cloneManager == null) return;
        _cloneManager.OnCreateClone();
        SetCloneButtonState(isRecording: true);
    }

    public void OnEndCreationPressed()
    {
        if (_cloneManager == null) return;
        _cloneManager.OnEndCreation();
        SetCloneButtonState(isRecording: false);
    }

    private void SetCloneButtonState(bool isRecording)
    {
        if (createCloneButton != null) createCloneButton.gameObject.SetActive(!isRecording);
        if (endCreationButton != null)  endCreationButton.gameObject.SetActive(isRecording);
    }

    // ── 일시정지 패널 ─────────────────────────────────────────────────────

    public void OnPausePressed(GameObject pausePanel)
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ClosePanel(GameObject panel)
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }

    // ── 씬 전환 ───────────────────────────────────────────────────────────

    public void OnRestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnStageSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(StageSelectSceneName);
    }

    // ── 스테이지 텍스트 ───────────────────────────────────────────────────

    /// <summary>
    /// StageManager에서 스테이지 시작 시 1회 호출.
    /// </summary>
    public void SetStageText(string text)
    {
        if (stageText != null)
            stageText.text = text;
    }
}
