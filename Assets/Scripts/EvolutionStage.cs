using UnityEngine;

// to store the info for each evolution stage (seed, sprout, full bloom)
// Right-click in Project > Create > Blooming > Evolution Stage to make one
[CreateAssetMenu(fileName = "EvolutionStage", menuName = "Blooming/Evolution Stage")]
public class EvolutionStage : ScriptableObject
{
    // just a name so we can tell them apart in the inspector
    public string stageName = "Unnamed Stage";
    public int stageIndex; // 0 = seed, 1 = sprout, 2 = full bloom

    // how much water the player needs to reach this stage
    public int waterRequired = 0;

    // stats that get applied to the player when they hit this stage
    public float maxHealth = 100f;
    public float moveSpeed = 5f;
    public float damage = 10f;

    // optional ability that unlocks at this stage
    public string abilityName = "";
    public GameObject abilityPrefab;

    // the player model to swap to when this stage is reached
    public GameObject visualPrefab;
}
