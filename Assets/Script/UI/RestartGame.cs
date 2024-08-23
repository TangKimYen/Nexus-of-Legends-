using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestartGame : MonoBehaviourPunCallbacks
{
    public Button restartButton; 
    public GameObject votePanel;
    public GameObject votingPanel;
    public GameObject settingPanel;
    public Button voteYesButton; 
    public Button voteNoButton;
    public TMP_Text voteYesText;
    public TMP_Text voteNoText;

    private int votesYes = 0;
    private int votesNo = 0;
    private bool isVoting = false;

    private void Start()
    {
        restartButton.onClick.AddListener(InitiateRestart);
        voteYesButton.onClick.AddListener(VoteYes);
        voteNoButton.onClick.AddListener(VoteNo);
        votePanel.SetActive(false);
    }

    public void InitiateRestart()
    {
        photonView.RPC("InitiateRestartVote", RpcTarget.All);
    }

    [PunRPC]
    public void InitiateRestartVote()
    {
        photonView.RPC("StartVote", RpcTarget.All);
    }

    [PunRPC]
    public void StartVote()
    {
        if (isVoting) return;
        Debug.Log("StartVote is called.");
        isVoting = true;
        settingPanel.transform.localScale = Vector3.zero;
        votePanel.SetActive(true);
        Debug.Log("votePanel set to active");
        votesYes = 0;
        votesNo = 0;
        votingPanel.SetActive(true);
        Debug.Log("votingPanel set to active");
        UpdateVoteTexts();  // Update vote text right after setting up the panel.
    }

    private void UpdateVoteTexts()
    {
        voteYesText.text = "Accept: " + votesYes + "/" + PhotonNetwork.PlayerList.Length;
        voteNoText.text = "Cancel: " + votesNo + "/" + PhotonNetwork.PlayerList.Length;
    }

    public void VoteYes()
    {
        photonView.RPC("CastVote", RpcTarget.All, true);
        votePanel.SetActive(false);
    }


    public void VoteNo()
    {
        photonView.RPC("CastVote", RpcTarget.All, false);
        votePanel.SetActive(false);
    }

    [PunRPC]
    public void CastVote(bool voteYes)
    {
        if (voteYes)
        {
            votesYes++;
        }
        else
        {
            votesNo++;
        }

        UpdateVoteTexts();
        if (votesYes + votesNo == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("CheckVoteResult", RpcTarget.All);
        }
    }

    [PunRPC]
    public void CheckVoteResult()
    {
        if (votesYes > votesNo)
        {
            votingPanel.SetActive(false);
            photonView.RPC("RestartMapGame", RpcTarget.All);
        }
        else
        {
            isVoting = false;
            votePanel.SetActive(false);
        }
    }

    [PunRPC]
    public void RestartMapGame()
    {
        string currentRoomName = PhotonNetwork.CurrentRoom.Name;
        string levelToLoad = currentRoomName.Replace("Floor ", "Map");
        PhotonNetwork.LoadLevel(levelToLoad);
    }
}
