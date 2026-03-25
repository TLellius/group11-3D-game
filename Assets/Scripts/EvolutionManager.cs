using UnityEngine;
using UnityEngine.Events;

// This script handles the whole evolution system
// must attach to the player 
public class EvolutionManager : MonoBehaviour
{
    // drag the 3 stage assets here in order so (seed first, full bloom last)
    public EvolutionStage[] stages;

    // the child object that holds the player's visible mesh
    public Transform visualRoot;

    // fires when the player evolves, useful for hooking up sound or particles later
    public UnityEvent<EvolutionStage> OnEvolved;

    // track how much water we've collected and which stage we're on
    public int WaterCollected { get; private set; }
    public int CurrentStageIndex { get; private set; } = 0;
    public EvolutionStage CurrentStage => stages[CurrentStageIndex];

    PlayerHealth _health;
    PlayerController _controller;
    GameObject _currentVisual;
    GameObject _currentAbility;

    void Awake()
    {
        // grab the other scripts on the player so we can update their values
        _health = GetComponent<PlayerHealth>();
        _controller = GetComponent<PlayerController>();

        if (stages == null || stages.Length == 0)
            Debug.LogError("No stages assigned! Drag the stage assets into the Stages array.");
    }

    void Start()
    {
        // apply the base stage right away so stats are set from the start
        ApplyStage(stages[0]);
    }

    // called by WaterOrb when the player touches a water pickup
    public void AddWater(int amount)
    {
        WaterCollected += amount;
        CheckEvolution();
    }

    void CheckEvolution()
    {
        // go through the stages from highest to lowest
        // and evolve to the first one the player qualifies for
        for (int i = stages.Length - 1; i > CurrentStageIndex; i--)
        {
            if (WaterCollected >= stages[i].waterRequired)
            {
                TriggerEvolution(i);
                return;
            }
        }
    }

    void TriggerEvolution(int newIndex)
    {
        CurrentStageIndex = newIndex;
        EvolutionStage stage = stages[newIndex];

        Debug.Log("Evolved to: " + stage.stageName);

        ApplyStage(stage);
        OnEvolved?.Invoke(stage);
    }

    void ApplyStage(EvolutionStage stage)
    {
        UpdateStats(stage);
        SwapVisual(stage);
        UnlockAbility(stage);
    }

    void UpdateStats(EvolutionStage stage)
    {
        if (_health != null)
        {
            _health.maxHealth = stage.maxHealth;
            _health.currentHealth = stage.maxHealth; // heal to full when evolving

            if (_health.healthSlider != null)
            {
                _health.healthSlider.maxValue = stage.maxHealth;
                _health.healthSlider.value = stage.maxHealth;
            }
        }

        if (_controller != null)
        {
            _controller.speed = stage.moveSpeed;
        }
    }

    void SwapVisual(EvolutionStage stage)
    {
        if (stage.visualPrefab == null) return;

        // destroy the old model and spawn the new one
        if (_currentVisual != null)
            Destroy(_currentVisual);

        Transform parent = visualRoot != null ? visualRoot : transform;
        _currentVisual = Instantiate(stage.visualPrefab, parent);
        _currentVisual.transform.localPosition = Vector3.zero;
        _currentVisual.transform.localRotation = Quaternion.identity;
    }

    void UnlockAbility(EvolutionStage stage)
    {
        if (stage.abilityPrefab == null) return;

        // remove old ability and attach the new one
        if (_currentAbility != null)
            Destroy(_currentAbility);

        _currentAbility = Instantiate(stage.abilityPrefab, transform);
        _currentAbility.name = stage.abilityName;
    }

    // call this from PlayerController when the player presses the attack button
    public void UseAbility()
    {
        if (_currentAbility == null) return;
        _currentAbility.GetComponent<IActivatable>()?.Activate();
    }
}

// any ability script should implement this interface so UseAbility() can call it
public interface IActivatable
{
    void Activate();
}
