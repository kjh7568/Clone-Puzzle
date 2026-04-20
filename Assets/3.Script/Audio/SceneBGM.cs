using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    [SerializeField] private BgmType bgmType = BgmType.Stage;

    private void Start() => AudioManager.Instance?.PlayBGM(bgmType);
}
