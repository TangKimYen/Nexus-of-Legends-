using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Hàm này s? ???c g?i khi b?n mu?n thoát kh?i game
    public void Quit()
    {
        // N?u ?ang ch?y trong Unity Editor, d?ng play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // N?u ?ang ch?y trên build th?c t?, thoát kh?i game
            Application.Quit();
#endif
    }
}
