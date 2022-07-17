using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniSoccerManager : MonoBehaviour
{
    private bool requestRestart = false;
    public static MiniSoccerManager Instance;

    [SerializeField]
    GameObject EndGameObject;

    [SerializeField]
    TextMeshProUGUI EndGameText;

    [SerializeField]
    GameObject[] PrefabPlayerTeams;

    [SerializeField]
    GameObject[] PrefabGoalAreas;

    [SerializeField]
    GameObject PrefabP1;

    [SerializeField]
    GameObject PrefabaAreaGolP1;

    [SerializeField]
    GameObject PrefabP2;

    [SerializeField]
    GameObject PrefabaAreaGolP2;

    [SerializeField]
    GameObject PrefabBall;

    [SerializeField]
    Transform[] Positions;

    [SerializeField]
    Transform[] GoalPointsPositions;

    int[] score;
    int maxGoals = 3;

    GameObject MyBody;
    GameObject MyGoal;
    GameObject TheBall;

    public bool Autostart = true;


    #region unity calls
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        EndGameText.text = "";
        if (Autostart)
            StartGame();

    }

    #endregion

    #region public calls
    //what to do if the player try to play again
    public void PlayAgain()
    {
        MatchMakingManager.reservedAutoMatchmaking = true;
        //to return to matchmaking
        StartCoroutine(AutoReconnect());
    }
    //callback goal made
    public void GoalMade(EventData photonEvent)
    {
        object[] data = (object[])photonEvent.CustomData;

        bool isPlayer1 = (bool)data[0];

        GoalMade(isPlayer1);
    }
    //call back if a player left the room
    public void OnPlayerLeft()
    {

        switch (MatchMakingManager.state)
        {
            case GameStatus.Play:
                Debug.Log("player left in game wins");
                UiSetEndGame("YOU");
                //to set winner
                break;
            case GameStatus.TeamSelection:
                Debug.Log("player left1");
                PlayAgain();
                break;
        }
    }

    #endregion

    #region private calls
    public void StartGame()
    {
        InitGame();
    }
    //init the game
    public void InitGame()
    {
        score = new int[2];
        score[0] = 0; //p1 player, the master client
        score[1] = 0;//p2 player, NOT the master client
        SetTextScore();

        if (PhotonNetwork.IsMasterClient)
        {
            int IDVisualPlaer = getIdBasedOnTeam();
            MyBody = PhotonNetwork.Instantiate(PrefabPlayerTeams[IDVisualPlaer].name, Positions[0].position, Positions[0].rotation);
            TheBall = PhotonNetwork.InstantiateRoomObject(PrefabBall.name, Positions[2].position, Positions[2].rotation);
        }
        else
        {

            int IDVisualPlaer = getIdBasedOnTeam();
            MyBody = PhotonNetwork.Instantiate(PrefabPlayerTeams[IDVisualPlaer].name, Positions[1].position, Positions[1].rotation);
        }

        InitGoalPoints();
    }
    int getIdBasedOnTeam()
    {
        int IDVisualPlaer = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            if (!TeamSelectorManager.Instance.P1Team.isSide1)
                IDVisualPlaer = 1;
        }
        else {
            if (!TeamSelectorManager.Instance.P2Team.isSide1)
                IDVisualPlaer = 1;
        }
        return IDVisualPlaer;
    }
    //set goal to the right points
    public void InitGoalPoints()
    {
        int positionId = Random.Range(0, GoalPointsPositions.Length);
        int IDVisualPlaer = getIdBasedOnTeam();
        MyGoal = PhotonNetwork.Instantiate(PrefabGoalAreas[IDVisualPlaer].name, GoalPointsPositions[positionId].position, GoalPointsPositions[positionId].rotation);

        MyGoal.GetComponentInChildren<GoalController>().IsPlayer1 = PhotonNetwork.IsMasterClient;
    }

    //called when a player made a goal
    private void GoalMade(bool isPlayer1)
    {
        Debug.Log("GOAL CALLED");
        if (isPlayer1)
            score[0]++;
        else
            score[1]++;
        SetTextScore();


        if ((score[1] >= maxGoals) || (score[0] >= maxGoals))
            EndGame(isPlayer1);
        else
            EndRound();

    }
    //update the score text
    private void SetTextScore()
    {
        EndGameText.text = "<size=25> P1</size> "+ score[0].ToString() + " - " + score[1].ToString() + " <size=25> P2</size>";
    }
    //and game behavour
    void EndGame(bool Isplayer1)
    {
        MatchMakingManager.state = GameStatus.GameEnded;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PhotonNetwork.DestroyPlayerObjects(player.ActorNumber, true);
        }

        string playerWins = "P1";
        if (!Isplayer1)
            playerWins = "P2";
        Debug.Log(playerWins + " WINS!!");

        UiSetEndGame(playerWins);

    }
    private void UiSetEndGame(string who)
    {
        EndGameObject.SetActive(true);
        EndGameText.text = who + " WINS!!";

    }
    //what to do at the end of each turn
    private void EndRound()
    {

        StartCoroutine(relocateAndStopVelocity());
    }

    IEnumerator relocateAndStopVelocity()
    {
        //relocate players
        if (PhotonNetwork.IsMasterClient)
            MyBody.transform.SetPositionAndRotation(Positions[0].position, Positions[0].rotation);
        else
            MyBody.transform.SetPositionAndRotation(Positions[1].position, Positions[1].rotation);
        //relocate goals
        int positionId = Random.Range(0, GoalPointsPositions.Length);
        MyGoal.transform.SetPositionAndRotation(GoalPointsPositions[positionId].position, GoalPointsPositions[positionId].rotation);

        //relocate ball
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
        if (PhotonNetwork.IsMasterClient)
            RelocateBall();

        yield return new WaitForSeconds(0.2f);
        //I want be REALLY sure the velocity will be resetted
        if (PhotonNetwork.IsMasterClient)
            RelocateBall();
       
    }
    private void RelocateBall()
    {
        TheBall.transform.SetPositionAndRotation(Positions[2].position, Positions[2].rotation);
        Rigidbody ballRigidbody = TheBall.GetComponent<Rigidbody>();
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
    }
    IEnumerator AutoReconnect()
    {
         if((PhotonNetwork.InRoom))
           PhotonNetwork.LeaveRoom();
           while (PhotonNetwork.InRoom)
               yield return null;

        SceneManager.Instance.LoadLoevel((int)ScenesID.ConnectingScene);
    }
    #endregion




  
}
