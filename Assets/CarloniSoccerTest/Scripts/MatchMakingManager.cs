using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MatchMakingManager : MonoBehaviour
{
    public static bool reservedAutoMatchmaking = false;

    public static GameStatus state;

    [SerializeField]
    private GameObject textMatchMaking;
    [SerializeField]
    private GameObject textConnecting;
    [SerializeField]
    private GameObject buttonConnect;

    public int PlayersNeededForStartPlay = 2;

    #region unity calls
    public void Start()
    {
        //for the start I supposed we are oflline, but if we have asked for an auto matchmaking lets see if we are connected, in a lobby, etc etc
        state = GameStatus.Offline;
        if (reservedAutoMatchmaking)
        {
            StartCoroutine(AutoReconnect());
        }
        else
            buttonConnect.SetActive(true);
    }
    #endregion

    #region public calls
    public void StartConnection()
    {
        SetUIConnecting();
        PhotonConnectorCustom.Instance.Connect();
        state = GameStatus.Connecting;
    }
    //when a player enter in the rom
    public void OnEnteredPlayer()
    {
        CheckIfPlayerEnoughtToStart();
    }
    //when I enter in the rom
    public void OnJoinedRoom()
    {
        state = GameStatus.Matchmackng;
        SetUIJoinedRoom();
        CheckIfPlayerEnoughtToStart();
    }
    #endregion

    #region UiSets
    private void SetUIConnecting()
    {
        buttonConnect.SetActive(false);
        textConnecting.SetActive(true);
    }
    private void SetUIJoinedRoom()
    {
        textMatchMaking.SetActive(true);
        textConnecting.SetActive(false);
    }
    #endregion

    #region private calls
    //reconnect automatically
    IEnumerator AutoReconnect()
    {
        //lets do action base on how we are in the server
        yield return new WaitForEndOfFrame();
        buttonConnect.SetActive(false);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.InLobby)
                PhotonConnectorCustom.Instance.JoinFirstRoomOrCreate();
            else
                PhotonConnectorCustom.Instance.JoinLobby();
        }
        else
            StartConnection();
    }

    private void LoadGameLevel()
    {
        SceneManager.Instance.LoadLoevel((int)ScenesID.GameScene);

    }
    //check if players is enought to start
    private void CheckIfPlayerEnoughtToStart()
    {
        if (PhotonNetwork.CurrentRoom.Players.Count == PlayersNeededForStartPlay)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            //we suppose that we start from here to choose the team in (this case)
            LoadGameLevel();
            MatchMakingManager.state = GameStatus.TeamSelection;
        }
    }

    #endregion

}
//all about the app status
public enum GameStatus
{
    Offline = 1,
    Connecting = 2,
    Matchmackng = 3,
    TeamSelection = 4,
    Play = 5,
    GameEnded = 6

}