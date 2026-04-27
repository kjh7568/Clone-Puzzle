using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    [SerializeField] private int stageNumber;
    [SerializeField] private TextMeshProUGUI label;

    public int StageNumber => stageNumber;

    public void SetLocked(bool locked)
    {
        var btn = GetComponent<Button>();
        btn.interactable = !locked;

        var cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = locked ? 0.4f : 1f;
    }
}
