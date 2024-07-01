using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealController : MonoBehaviour
{
    [SerializeField] private int healAmount;

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

    public int GetHealAmount()
    {
        return healAmount;
    }

    public void SetHealAmount(int amount)
    {
        healAmount = amount;
    }
}
