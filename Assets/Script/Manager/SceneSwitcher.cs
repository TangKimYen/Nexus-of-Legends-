using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // H�m ?? chuy?n ??n CreditScene
    public void SwitchToCreditScene()
    {
        SceneManager.LoadScene("ViewCredit");
    }
    // H�m ?? chuy?n ??n MainScene
    public void SwitchToMainScene()
    {
        SceneManager.LoadScene("TitleScreen");
    }
    // H�m ?? chuy?n ??n StorylineScene
    public void SwitchToStorylineScene()
    {
        SceneManager.LoadScene("ViewStoryline");
    }
}
