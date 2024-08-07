using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealController : MonoBehaviour
{
    [SerializeField] private int baseHealAmount;
    private PlayerStats playerStats;

    public static HealController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Holds the HealController's game object when transitioning scenes
        }
        else
        {
            Destroy(gameObject); // Destroy new instances if another instance already exists
        }
    }

    private void Start()
    {
        playerStats = PlayerStats.Instance;
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats instance is not found. Ensure that PlayerStats is initialized.");
        }
    }

    public int GetHealAmount()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats instance is not found. Ensure that PlayerStats is initialized.");
            return baseHealAmount; // Return base heal amount if playerStats is not available
        }

        return Mathf.FloorToInt(baseHealAmount + playerStats.GetIntellect() * 1.5f); // Example calculation
    }

    public void SetBaseHealAmount(int amount)
    {
        baseHealAmount = amount;
    }


}
