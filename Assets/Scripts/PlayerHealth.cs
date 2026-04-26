using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public Slider healthSlider;

    private PlayerController env;

    public EvolutionManager evolutionManager;

    public EvolutionStage baseStage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        //env = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10f);
        }

        healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth, Time.deltaTime * 10);
    }

    public void updateHealth(float max)
    {
        maxHealth = max;
        currentHealth = maxHealth;
        //Debug.Log("Health updated: " + currentHealth + "/" + maxHealth);
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(float amount)
    {
        //Debug.Log("Before damage: " + currentHealth);
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        //Debug.Log("After damage: " + currentHealth);
        healthSlider.value = currentHealth;

        if (currentHealth <= 50f)
        {
            if (!(evolutionManager.CurrentStageIndex == 0))
            {
                Debug.Log(evolutionManager.CurrentStageIndex);
                //float actualCurrentHealth = currentHealth;
                //evolutionManager.ApplyStage(baseStage);
                //currentHealth = actualCurrentHealth;
                evolutionManager.LowerWater(30);

                evolutionManager.ReverseStage(baseStage);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Freezes the game
        Time.timeScale = 0f;
    }
}
