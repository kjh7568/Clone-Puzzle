using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Clone Puzzle/Stage Data")]
public class StageData : ScriptableObject
{
    [Min(1)]
    public int maxCloneCount = 3;
}
