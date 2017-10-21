using UnityEngine;

public enum PlayerChildren { Evolutions };
public class Player : MonoBehaviour {

    Rigidbody rb;
    bool canDoubleJump = false;

    [Header("Collectables")]
    [SerializeField] int[] collectables;

    public uint activeEvolutions = 0;

    public Transform respawnPoint;
    public GameObject cameraReference;

    public Rigidbody Rb
    {
        get
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();
            return rb;
        }

        set
        {
            rb = value;
        }
    }

    public bool CanDoubleJump
    {
        get
        {
            return canDoubleJump;
        }

        set
        {
            canDoubleJump = value;
        }
    }

    public int[] Collectables
    {
        get
        {
            return collectables;
        }

        set
        {
            collectables = value;
        }
    }

    public void UpdateCollectableValue(CollectableType type, int pickedValue)
    {
        collectables[(int)type] = Mathf.Clamp(collectables[(int)type] + pickedValue, 0, Utils.GetMaxValueForCollectable(type));
        if (!Utils.IsAnEvolutionCollectable(type))
            return;

        EvolutionCheck(type);
    }

    void EvolutionCheck(CollectableType type)
    {
        if (GameManager.CurrentGameMode.evolutionMode == EvolutionMode.GrabCollectableAndAutoEvolve)
        {
            Evolution evolution = GameManager.EvolutionManager.GetEvolutionByCollectableType(type);
            if (collectables[(int)type] >= evolution.Cost)
                EvolveGameplay1(evolution);
        }
        else if (GameManager.CurrentGameMode.evolutionMode == EvolutionMode.GrabEvolution)
        {
            if (activeEvolutions == 0)
            {
                Evolution evolution = GameManager.EvolutionManager.GetEvolutionByCollectableType(type);
                PermanentEvolution(evolution);
            }
        }
    }

    // GAMEPLAY TEST 1: all of this should be in an Evolution class handling all evolution parameters (+ we should be able to pickup collectables and "refresh" an evolution indefinitely)
    private void EvolveGameplay1(Evolution evolution)
    {
        GameManager.EvolutionManager.AddEvolutionComponent(gameObject, evolution);
        collectables[(int)evolution.AssociatedCollectable] -= evolution.Cost;
    }

    public void EvolveGameplay2(Evolution evolution)
    {
        GameManager.EvolutionManager.AddEvolutionComponent(gameObject, evolution);
        collectables[0] -= evolution.Cost;
    }

    private void PermanentEvolution(Evolution evolution)
    {
        GameManager.EvolutionManager.AddEvolutionComponent(gameObject, evolution, true);
    }

    void Start () {
        rb = GetComponent<Rigidbody>();
        collectables = new int[(int)CollectableType.Size];
    }

}
