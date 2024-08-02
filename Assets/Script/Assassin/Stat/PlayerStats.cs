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
    [SerializeField] private AudioSource levelUpSound;

    private DatabaseReference databaseReference;

    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        if (playerUI == null)
        {
            Debug.LogError("PlayerUI component is not found on this GameObject.");
            return;
        }
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
}
