using UnityEngine;

//for water pickup
[RequireComponent(typeof(Collider))]
public class WaterPickup : MonoBehaviour
{
    public int waterAmount = 1;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        EvolutionManager evo = other.GetComponent<EvolutionManager>();
        if (evo == null) return;

        evo.AddWater(waterAmount);
        Destroy(gameObject);
    }
}
