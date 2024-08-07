using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundChanger : MonoBehaviour
{
    public Sprite[] backgrounds;  
    public Image targetImage; 

    void Start()
    {
        if (backgrounds.Length > 0)
        {
            ChangeBackground();
        }
        else
        {
            Debug.LogError("No backgrounds assigned!");
        }
    }

    public void ChangeBackground()
    {
        if (targetImage != null && backgrounds.Length > 0)
        {
            int randomIndex = Random.Range(0, backgrounds.Length);
            targetImage.sprite = backgrounds[randomIndex];
        }
        else
        {
            Debug.LogError("Target Image or backgrounds array is not assigned!");
        }
    }
}
