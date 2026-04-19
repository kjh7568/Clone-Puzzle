using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Clone Puzzle/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("Clone")]
    [Min(1)]
    public int maxCloneCount = 3;

    [Header("Lifespan")]
    public float maxLifespan = 100f;
    public float moveCostPerSecond = 10f;
    public float jumpCost = 5f;
    public float carryCost = 8f;
}
