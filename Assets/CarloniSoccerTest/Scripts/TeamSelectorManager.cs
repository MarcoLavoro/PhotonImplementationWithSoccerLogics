using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeamSelectorManager : MonoBehaviour
{
    public static TeamSelectorManager Instance;
    [SerializeField]
    private GameObject buttonStart;

    [SerializeField]
    private GameObject canvasMain;

    [SerializeField]
    private TextMeshProUGUI textWhoAmI;

    public PlayerTeam P1Team = null;
    public PlayerTeam P2Team = null;
    public GameObject[] textPlayers;
    public Transform[] positionXReference;

    public static byte ChangeTeamEventCode = 1;
    public static byte StartGameRequest = 2;

    #region unity calls
    private void Awake()
    {
        Instance = this;
        //set who am I
        UiSetWhatPlayerYouAre();
    }
    #endregion

    #region public calls
    //send event for change team
    public void SetTeam(int team)
    {
        bool isTeamLeft = (team == 0);

        object[] content = new object[] { PhotonNetwork.IsMasterClient, isTeamLeft }; // Array contains the target position and the IDs of the selected units
        EventManager.Instance.SendEventToAll(content, EventsCodes.ChangeTeamEventCode);
    }
    //send event to start the game (we supposed this button is enabled ONLY if the players are on opposite team)
    public void SendStartGame()
    {
        bool sended = true;

        object[] content = new object[] { sended }; // Array contains the target position and the IDs of the selected units
        EventManager.Instance.SendEventToAll(content, EventsCodes.StartGameRequest);
    }

    //action called by the event manager to actually change team
    public void ChangeTeam(EventData photonEvent)
    {
        object[] data = (object[])photonEvent.CustomData;

        bool wasMaster = (bool)data[0];
        bool teamSide = (bool)data[1];

        Debug.Log("The master client = " + wasMaster + " choose team left = " + teamSide);

        SetTextPlayer(wasMaster, teamSide);

        if (CheckSelection())
            buttonStart.SetActive(true);
        else
            buttonStart.SetActive(false);
    }

    //action called by the event manager to start the game
    public void StartGame()
    {
        Debug.Log("start game");
        canvasMain.SetActive(false);
        MiniSoccerManager.Instance.StartGame();
        MatchMakingManager.state = GameStatus.Play;

    }
    #endregion

    #region private calls

    private void UiSetWhatPlayerYouAre()
    {
        string who = "P1";
        if (!PhotonNetwork.IsMasterClient)
            who = "P2";
        textWhoAmI.text = who;
    }
    //set move the player text from a team position to another one
    private void SetTextPlayer(bool ismaster, bool IsSide1)
    {
        GameObject toMove;
        //if is master is supposed to be P1
        if (ismaster)
            toMove = textPlayers[0];
        else
            toMove = textPlayers[1];

        toMove.SetActive(true);
        //if  IsSide1 is supposed to be left team
        if (IsSide1)
            toMove.transform.position = new Vector3(positionXReference[0].position.x, toMove.transform.position.y, toMove.transform.position.z);
        else
            toMove.transform.position = new Vector3(positionXReference[1].position.x, toMove.transform.position.y, toMove.transform.position.z);

        if (ismaster)
        {
            if (P1Team == null)
                P1Team = new PlayerTeam(IsSide1);
            else
                P1Team.isSide1 = IsSide1;

        }
        else
        {
            if (P2Team == null)
                P2Team = new PlayerTeam(IsSide1);
            else
                P2Team.isSide1 = IsSide1;
        }
    }
    //check if player are on opposite team
    private bool CheckSelection()
    {
        if ((P1Team == null) || (P2Team == null))
            return false;

        if (P1Team.isSide1 == P2Team.isSide1)
            return false;

        return true;
    }
    #endregion

}

public class PlayerTeam
{
    public bool isSide1 = false;

    public PlayerTeam(bool _isSide1)
    {
        isSide1 = _isSide1;
    }
}
