using Firebase.Database;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    public DatabaseReference DatabaseReference { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase Initialized");
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }
}
