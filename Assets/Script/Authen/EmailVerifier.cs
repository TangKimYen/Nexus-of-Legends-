using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class EmailVerifier : MonoBehaviour
{
    private string zeroBounceApiKey = "24cad90242f2465ab6b1e6a72d26f3a5"; // Thay thế bằng API key của bạn
    private string zeroBounceApiUrl = "https://api.zerobounce.net/v2/validate";

    public TMP_InputField emailInput;
    public TextMeshProUGUI messageText; // Để hiển thị thông báo cho người dùng

    private Color successColor;
    private Color errorColor;

    void Start()
    {
        // Chuyển đổi mã màu hex sang Color
        ColorUtility.TryParseHtmlString("#007213", out successColor); // Màu xanh lục
        ColorUtility.TryParseHtmlString("#C02E31", out errorColor);   // Màu đỏ
    }

    public IEnumerator VerifyEmail(string email, System.Action<bool> callback)
    {
        string url = $"{zeroBounceApiUrl}?api_key={zeroBounceApiKey}&email={email}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                messageText.text = "Error verifying email: " + www.error;
                messageText.color = errorColor;
                callback(false);
            }
            else
            {
                ZeroBounceResponse response = JsonUtility.FromJson<ZeroBounceResponse>(www.downloadHandler.text);
                if (response.status == "valid")
                {
                    callback(true);
                }
                else
                {
                    messageText.text = "Email is invalid: " + response.status;
                    messageText.color = errorColor;
                    callback(false);
                }
            }
        }
    }
}

[System.Serializable]
public class ZeroBounceResponse
{
    public string address;
    public string status;
    public string sub_status;
    public string account;
    public string domain;
    public string did_you_mean;
    public string domain_age_days;
    public string free_email;
    public string mx_found;
    public string mx_record;
    public string smtp_provider;
    public string first_name;
    public string last_name;
    public string gender;
    public string country;
    public string region;
    public string city;
    public string zipcode;
    public string processed_at;
}