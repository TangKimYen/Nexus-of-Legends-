using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviourPunCallbacks
{
    private PlayerUI playerUI;
    [SerializeField] private Transform spamPoint;
    [SerializeField] private GameObject[] LevelUpEffect;
    public PlayerCurrentStats playerCurrentStat;
    private DatabaseReference reference;
    private DatabaseReference currentStatReference;
    private DatabaseReference baseStatReference;
    public PlayerBaseStat playerBaseStat;
    private float strength;
    private float intellect;
    public int damage;
    [SerializeField] private AudioSource levelUpSound;

    public static PlayerStats Instance;

    public int level;
    public float currentExp;
    public float expToNextLevel;
    public float gold;
    public int baseExp = 100;
    public float growthFactor = 1.5f;

    private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else if (Instance != this)
    {
        Instance = this;
    }
}
    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        reference = FirebaseDatabase.DefaultInstance.GetReference("players").Child(PlayerData.instance.username);
        currentStatReference = FirebaseDatabase.DefaultInstance.GetReference("PlayerCurrentStat").Child(PlayerData.instance.username);
        baseStatReference = FirebaseDatabase.DefaultInstance.GetReference("PlayerBaseStat").Child(PlayerData.instance.username);
        LoadData();
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
        SavePlayerData();
        UpdateExpUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        SavePlayerData();
        UpdateGoldUI();
    }

    void LevelUp()
    {
        currentExp -= expToNextLevel;
        level++;
        expToNextLevel = CalculateExpToNextLevel(level);
        SavePlayerData();
        UpdateExpUI();

        levelUpSound.Play();
        LevelUpEffects();
        photonView.RPC("LevelUpEffectRPC", RpcTarget.All, spamPoint.position, transform.localScale.x);
        IncreaseBaseStats();
    }

    public void LevelUpEffects()
    {
        int effectIndex = FindLevelUpEffect();
        LevelUpEffect[effectIndex].SetActive(true);
        LevelUpEffect[effectIndex].transform.position = spamPoint.position;
        LevelUpEffect[effectIndex].GetComponent<SkillHit>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int FindLevelUpEffect()
    {
        for (int i = 0; i < LevelUpEffect.Length; i++)
        {
            if (!LevelUpEffect[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    [PunRPC]
    private void LevelUpEffectRPC(Vector3 levelUpPosition, float direction)
    {
        int effectIndex = FindLevelUpEffect();
        LevelUpEffect[effectIndex].transform.position = levelUpPosition;
        LevelUpEffect[effectIndex].GetComponent<SkillHit>().SetDirection(Mathf.Sign(direction));
    }

    public void LoadData()
    {
        StartCoroutine(LoadDataEnum());
    }
    IEnumerator LoadDataEnum()
    {
        if (PlayerData.instance == null)
        {
            Debug.LogError("PlayerData.instance is null. Ensure that PlayerData is initialized.");
            yield break;
        }

        Debug.Log("Loading data for: " + PlayerData.instance.username);

        // Load level, exp, and gold from the "players" node
        var serverPlayerData = reference.GetValueAsync();
        yield return new WaitUntil(predicate: () => serverPlayerData.IsCompleted);

        DataSnapshot playerSnapshot = serverPlayerData.Result;
        if (playerSnapshot.Exists)
        {
            level = int.Parse(playerSnapshot.Child("level").Value.ToString());
            currentExp = float.Parse(playerSnapshot.Child("exp").Value.ToString());
            gold = float.Parse(playerSnapshot.Child("gold").Value.ToString());
            expToNextLevel = CalculateExpToNextLevel(level);
        }
        else
        {
            Debug.LogWarning("Player data does not exist in Firebase.");
        }

        // Load strength and intellect from the "PlayerCurrentStat" node
        var serverCurrentData = currentStatReference.GetValueAsync();
        yield return new WaitUntil(predicate: () => serverCurrentData.IsCompleted);

        DataSnapshot currentSnapshot = serverCurrentData.Result;
        string jsonCurrentData = currentSnapshot.GetRawJsonValue();

        if (jsonCurrentData != null)
        {
            playerCurrentStat = JsonUtility.FromJson<PlayerCurrentStats>(jsonCurrentData);
            strength = playerCurrentStat.currentStrength;
            intellect = playerCurrentStat.currentIntellect;
        }
        else
        {
            Debug.LogWarning("Player current stat data does not exist in Firebase.");
        }

        // Load base stats from the "PlayerBaseStat" node
        var serverBaseStatData = baseStatReference.GetValueAsync();
        yield return new WaitUntil(predicate: () => serverBaseStatData.IsCompleted);

        DataSnapshot baseStatSnapshot = serverBaseStatData.Result;
        if (baseStatSnapshot.Exists)
        {
            playerBaseStat = JsonUtility.FromJson<PlayerBaseStat>(baseStatSnapshot.GetRawJsonValue());
        }
        else
        {
            Debug.LogWarning("Player base stats do not exist in Firebase.");
        }

        UpdateExpUI();
        UpdateGoldUI();
    }

    public void SavePlayerData()
    {
        reference.Child("level").SetValueAsync(level);
        reference.Child("exp").SetValueAsync(currentExp);
        reference.Child("gold").SetValueAsync(gold);
    }

    public float CalculateExpToNextLevel(int level)
    {
        return Mathf.FloorToInt(baseExp * Mathf.Pow(level, growthFactor));
    }

    public void UpdateExpUI()
    {
        if (playerUI != null)
        {
            playerUI.UpdateExpUI(currentExp, expToNextLevel, level);
        }
    }

    public void UpdateGoldUI()
    {
        if (playerUI != null)
        {
            playerUI.UpdateGoldUI(gold);
        }
    }

    public int CalculateDamage()
    {
        if (playerCurrentStat == null)
        {
            return 0;
        }

        damage = Mathf.FloorToInt(strength + intellect * 2);
        Debug.Log(damage);
        return damage;
    }

    public float GetIntellect()
    {
        return intellect;
    }

    private void IncreaseBaseStats()
    {
        playerBaseStat.baseStrength += 5; 
        playerBaseStat.baseIntellect += 3;
        playerBaseStat.baseDefense += 0.5f; 
        playerBaseStat.baseBlood += 20;
        playerBaseStat.baseMovement += 0.5f;
        playerBaseStat.baseAttackSpeed += 0.2f;

        SaveBaseStats();
    }

    public void SaveBaseStats()
    {
        string jsonBaseStats = JsonUtility.ToJson(playerBaseStat);
        baseStatReference.SetRawJsonValueAsync(jsonBaseStats);
    }
}
