using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldGemDisplay : MonoBehaviour
{
    public TMP_Text gemText;
    public TMP_Text goldText;

    private DatabaseReference reference;

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.GetReference("players").Child(PlayerData.instance.username);
        LoadPlayerData();
    }

    void LoadPlayerData()
    {
        reference.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Unable to retrieve player data from Firebase: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    UpdateUI(snapshot);
                    // Set up a listener for real-time updates
                    reference.ValueChanged += OnValueChanged;
                }
                else
                {
                    Debug.LogWarning("Player data does not exist in Firebase.");
                }
            }
        });
    }

    private void OnValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.DatabaseError != null)
        {
            Debug.LogError("Error retrieving player data: " + e.DatabaseError.Message);
            return;
        }
        if (e.Snapshot.Exists)
        {
            UpdateUI(e.Snapshot);
        }
    }

    private void UpdateUI(DataSnapshot snapshot)
    {
        try
        {
            float gold = float.Parse(snapshot.Child("gold").Value.ToString());
            float gem = float.Parse(snapshot.Child("gem").Value.ToString());

            goldText.text = gold.ToString();
            gemText.text = gem.ToString();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error processing player data: " + ex.Message);
        }
    }

    private void OnDestroy()
    {
        // Remove the listener when this object is destroyed
        if (reference != null)
        {
            reference.ValueChanged -= OnValueChanged;
        }
    }
}
