using Firebase.Database;
using NOL.CharacterStats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPanel : MonoBehaviour
{
    [SerializeField] StatDisplay[] statDisplays;
    [SerializeField] string[] statNames;

    private CharacterStat[] stats;
    private DatabaseReference dbRef;

    private void OnValidate()
    {
        statDisplays = GetComponentsInChildren<StatDisplay>();
        UpdateStatNames();
    }

    private void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SetStat(params CharacterStat[] charStats)
    {
        stats = charStats;

        if (stats.Length > statDisplays.Length)
        {
            Debug.LogError("Not enough space to display stat!");
            return;
        }

        for (int i = 0; i < statDisplays.Length; i++)
        {
            statDisplays[i].gameObject.SetActive(i < stats.Length);
        }
    }

    public void UpdateStatValues()
    {
        for (int i = 0; i < stats.Length; i++)
        {
            float roundedValue = Mathf.Round(stats[i].Value);
            statDisplays[i].ValueText.text = roundedValue.ToString();
        }
        StartCoroutine(SavePlayerCurrentStatsCoroutine());
    }

    private IEnumerator SavePlayerCurrentStatsCoroutine()
    {
        yield return new WaitForEndOfFrame();

        SavePlayerCurrentStats();
    }

    public void UpdateStatNames()
    {
        for (int i = 0; i < statNames.Length; i++)
        {
            statDisplays[i].NameText.text = statNames[i];
        }
    }

    private void SavePlayerCurrentStats()
    {
        if (dbRef == null)
        {
            Debug.LogError("dbRef is null. Ensure that dbRef is initialized.");
            return;
        }

        if (stats == null || stats.Length < 6)
        {
            Debug.LogError("Stats array is null or does not contain enough elements.");
            return;
        }

        if (PlayerData.instance == null || string.IsNullOrEmpty(PlayerData.instance.username))
        {
            Debug.LogError("PlayerData.instance is null or username is empty.");
            return;
        }

        if (PlayerData.instance != null && !string.IsNullOrEmpty(PlayerData.instance.username))
        {
            string username = PlayerData.instance.username;
            PlayerCurrentStats currentStats = new PlayerCurrentStats
            {
                currentStrength = Mathf.Round(stats[0].Value),
                currentIntellect = Mathf.Round(stats[1].Value),
                currentDefense = Mathf.Round(stats[2].Value),
                currentBlood = Mathf.Round(stats[3].Value),
                currentMovement = Mathf.Round(stats[4].Value),
                currentAttackSpeed = Mathf.Round(stats[5].Value)
            };

            string json = JsonUtility.ToJson(currentStats);
            dbRef.Child("PlayerCurrentStat").Child(username).SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Player current stats saved to Firebase successfully.");
                }
                else
                {
                    Debug.LogError("Failed to save player current stats to Firebase: " + task.Exception);
                }
            });
        }
        else
        {
            Debug.LogError("PlayerData.instance is null or username is empty.");
        }
    }
}
