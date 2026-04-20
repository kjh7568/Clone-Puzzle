using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Clone Puzzle/Audio Data")]
public class AudioData : ScriptableObject
{
    [Header("BGM")]
    public AudioClip bgmStage;
    public AudioClip bgmMenu;
    public AudioClip bgmStageSelect;

    [Header("SFX")]
    public AudioClip sfxJumpLand;
    public AudioClip sfxCarryPickup;
    public AudioClip sfxCarryDrop;
    public AudioClip sfxButtonClick;
    public AudioClip sfxButtonPress;
    public AudioClip sfxCloneCreate;
    public AudioClip sfxCloneEnd;
    public AudioClip sfxCloneReset;
    public AudioClip sfxStageCleared;
    public AudioClip sfxDoorOpen;
    public AudioClip sfxScaleBalance;
}
