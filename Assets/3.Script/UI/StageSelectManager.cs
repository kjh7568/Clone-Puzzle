using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Image fadePanel;

    private const float FadeDuration = 1f;

    private StageButton[] _buttons;

    private void Start()
    {
        _buttons = buttonContainer.GetComponentsInChildren<StageButton>(true);
        RefreshButtons();

        if (fadePanel != null)
        {
            SetFadeAlpha(0f);
            fadePanel.gameObject.SetActive(false);
        }
    }

    public void LoadStage(int stageNumber)
    {
        StartCoroutine(FadeAndLoad("Stage " + stageNumber));
    }

    public void LoadTitleScene()
    {
        SceneManager.LoadScene("Title Scene");
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            float elapsed = 0f;
            while (elapsed < FadeDuration)
            {
                elapsed += Time.deltaTime;
                SetFadeAlpha(Mathf.Clamp01(elapsed / FadeDuration));
                yield return null;
            }
            SetFadeAlpha(1f);
        }
        SceneManager.LoadScene(sceneName);
    }

    private void SetFadeAlpha(float alpha)
    {
        Color c = fadePanel.color;
        c.a = alpha;
        fadePanel.color = c;
    }

    public bool IsStageUnlocked(int stageNumber)
    {
        return StageProgressManager.Instance.IsUnlocked(stageNumber);
    }

    public void RefreshButtons()
    {
        if (_buttons == null) return;
        foreach (StageButton btn in _buttons)
            btn.SetLocked(!IsStageUnlocked(btn.StageNumber));
    }
}
