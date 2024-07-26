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

    // G?i ph??ng th?c này khi có tin nh?n m?i
    public void OnNewMessage()
    {
        ScrollToBottom();
    }
}
