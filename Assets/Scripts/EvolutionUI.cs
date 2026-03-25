using UnityEngine;
using TMPro;

// attach this to a Canvas GameObject to show water count and current stage
public class EvolutionUI : MonoBehaviour
{
    public EvolutionManager evolutionManager;

    public TextMeshProUGUI waterLabel;
    public TextMeshProUGUI stageLabel;

    void Update()
    {
        if (evolutionManager == null) return;

        if (waterLabel != null)
            waterLabel.text = "Water: " + evolutionManager.WaterCollected;

        if (stageLabel != null)
            stageLabel.text = "Stage: " + evolutionManager.CurrentStage.stageName;
    }
}
