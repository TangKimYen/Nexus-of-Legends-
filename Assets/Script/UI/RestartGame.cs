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
    public Button voteYesButton; 
    public Button voteNoButton;
    public TMP_Text voteYesText;
    public TMP_Text voteNoText;

    private int votesYes = 0;
    private int votesNo = 0;
    private bool isVoting = false;

    private void Start()
    {
        restartButton.onClick.AddListener(InitiateRestartVote);
        voteYesButton.onClick.AddListener(VoteYes);
        voteNoButton.onClick.AddListener(VoteNo);
        votePanel.SetActive(false);
    }

    public void InitiateRestartVote()
    {
        if (isVoting) return;

        photonView.RPC("StartVote", RpcTarget.All);
    }

    [PunRPC]
    public void StartVote()
    {
        isVoting = true;
        votePanel.SetActive(true);
        votesYes = 0;
        votesNo = 0;
        votingPanel.SetActive(true);
        voteYesText.text = "Accept: 0/" + PhotonNetwork.PlayerList.Length;
        voteNoText.text = "Cancel: 0/" + PhotonNetwork.PlayerList.Length;
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

        voteYesText.text = "Accept: " + votesYes + "/" + PhotonNetwork.PlayerList.Length;
        voteNoText.text = "Cancel: " + votesNo + "/" + PhotonNetwork.PlayerList.Length;
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
        if(PhotonNetwork.CurrentRoom.Name == "Floor 6")
        {
            PhotonNetwork.LoadLevel("Map6");
        }
        else
        {
            PhotonNetwork.LoadLevel("Map9");
        }
    }
}
