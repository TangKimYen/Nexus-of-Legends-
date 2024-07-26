using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect;

    public void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; // Cu?n xu?ng cu?i
    }

    // G?i ph??ng th?c n�y khi c� tin nh?n m?i
    public void OnNewMessage()
    {
        ScrollToBottom();
    }
}
