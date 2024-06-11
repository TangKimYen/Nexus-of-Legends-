using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static AudioManager instance;
    public AudioSource backgroundMusic;
    private bool isMuted = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        backgroundMusic.mute = isMuted;
    }

    public bool IsMuted()
    {
        return isMuted;
    }
}
