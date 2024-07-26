using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ProfileUI : MonoBehaviour
{
    public TMP_Text playerName;
    public GameObject AvatarAssassin;
    public GameObject AvatarWarrior;
    public GameObject AvatarMagican;
    public GameObject AvatarArcher;
    // Start is called before the first frame update
    void Start()
    {
        playerName.text = PlayerData.instance.username;
        switch (PlayerData.instance.characterId)
        {
            case "c01":
                AvatarAssassin.SetActive(true);
                break;
            case "c02":
                AvatarArcher.SetActive(true);
                break;
            case "c03":
                AvatarWarrior.SetActive(true);
                break;
            case "c04":
                AvatarMagican.SetActive(true);
                break;
            default:
                AvatarAssassin.SetActive(true);
                break;
        }
    }
}
