using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Hàm ?? chuy?n ??n CreditScene
    public void SwitchToCreditScene()
    {
        SceneManager.LoadScene("ViewCredit");
    }
    // Hàm ?? chuy?n ??n MainScene
    public void SwitchToMainScene()
    {
        SceneManager.LoadScene("TitleScreen");
    }
    // Hàm ?? chuy?n ??n StorylineScene
    public void SwitchToStorylineScene()
    {
        SceneManager.LoadScene("ViewStoryline");
    }
}
