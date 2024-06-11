using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // H�m n�y s? ???c g?i khi b?n mu?n tho�t kh?i game
    public void Quit()
    {
        // N?u ?ang ch?y trong Unity Editor, d?ng play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // N?u ?ang ch?y tr�n build th?c t?, tho�t kh?i game
            Application.Quit();
#endif
    }
}
