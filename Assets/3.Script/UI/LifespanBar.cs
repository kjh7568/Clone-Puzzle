using UnityEngine;
using UnityEngine.UI;

public class LifespanBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private Actor _target;

    private void Start()
    {
        _target = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if (_target == null || fillImage == null) return;
        fillImage.fillAmount = _target.LifespanRatio;
    }
}
