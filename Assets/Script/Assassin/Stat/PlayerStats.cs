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
    private float strength;
    private float intellect;
    public int damage;
    [SerializeField] private AudioSource levelUpSound;

    public static PlayerStats Instance;
    private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else if (Instance != this)
    {
        Destroy(gameObject);
    }
}
    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        if (playerUI == null)
        {
            Debug.LogError("PlayerUI component is not found on this GameObject.");
            return;
        }
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        LoadData();
    }

    public void AddExp(int amount)
    {
        if (PlayerUI.Instance == null)
        {
            Debug.LogError("PlayerUI.Instance is null. Ensure that PlayerUI is initialized.");
            return;
        }
        PlayerUI.Instance.currentExp += amount;
        if (PlayerUI.Instance.currentExp >= PlayerUI.Instance.expToNextLevel)
        {
            LevelUp();
        }
        PlayerUI.Instance.SavePlayerData();
        playerUI.UpdateExpUI(PlayerUI.Instance.currentExp, PlayerUI.Instance.expToNextLevel, PlayerUI.Instance.level);
    }

    public void AddGold(int amount)
    {
        if (PlayerUI.Instance == null)
        {
            Debug.LogError("PlayerUI.Instance is null. Ensure that PlayerUI is initialized.");
            return;
        }

        PlayerUI.Instance.gold += amount;
        playerUI.UpdateGoldUI();
        PlayerUI.Instance.SavePlayerData();
    }

    void LevelUp()
    {
        if (PlayerUI.Instance == null)
        {
            Debug.LogError("PlayerUI.Instance is null. Ensure that PlayerUI is initialized.");
            return;
        }

        PlayerUI.Instance.currentExp -= PlayerUI.Instance.expToNextLevel;
        PlayerUI.Instance.level++;
        PlayerUI.Instance.expToNextLevel = PlayerUI.Instance.CalculateExpToNextLevel(PlayerUI.Instance.level);
        PlayerUI.Instance.SavePlayerData();
        playerUI.UpdateExpUI(PlayerUI.Instance.currentExp, PlayerUI.Instance.expToNextLevel, PlayerUI.Instance.level);

        levelUpSound.Play();
        LevelUpEffects();
        photonView.RPC("LevelUpEffectRPC", RpcTarget.All, spamPoint.position, transform.localScale.x);
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
        var serverCurrentData = reference.Child("PlayerCurrentStat").Child(PlayerData.instance.username).GetValueAsync();
        yield return new WaitUntil(predicate: () => serverCurrentData.IsCompleted);

        Debug.Log("Quá trình tải stat hiện tại đã hoàn tất!");

        DataSnapshot currentSnapshot = serverCurrentData.Result;
        string jsonCurrentData = currentSnapshot.GetRawJsonValue();

        if (jsonCurrentData != null)
        {
            Debug.Log("Dữ liệu stat hiện tại được tìm thấy.");
            playerCurrentStat = JsonUtility.FromJson<PlayerCurrentStats>(jsonCurrentData);
            strength = playerCurrentStat.currentStrength;
            intellect = playerCurrentStat.currentIntellect;
        }
        else
        {
            Debug.Log("Dữ liệu stat hiện tại không được tìm thấy.");
        }
    }

    public int CalculateDamage()
    {
        if (playerCurrentStat == null)
        {
            Debug.LogError("PlayerCurrentStats chưa được tải.");
            return 0;
        }

        damage = Mathf.FloorToInt(strength + intellect * 2);
        Debug.Log(damage);
        return damage;
    }
}
