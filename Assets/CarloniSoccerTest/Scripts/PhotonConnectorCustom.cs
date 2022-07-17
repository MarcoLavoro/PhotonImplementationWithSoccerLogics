using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class PhotonConnectorCustom : MonoBehaviour
{
    //generic custon photon connector with unity event, this allow to be as flexible as we want
    public static PhotonConnectorCustom Instance;
    [SerializeField]
    private bool dontDestroyOnLoad= true;

    [SerializeField]
    private byte MaxPlayers = 2;
    public enum AutoLogin { AutoConnectOnly, ConnectAndLobby, LobbyAndRoom, None };

    public AutoLogin AutologinAtStart;
    public LobbyCustom lobby;

    public UnityEvent OnEnteredPlayer;
    public UnityEvent OnLeftPlayer;

    public PlayerEvent OnEnteredPlayerVariable;
    public PlayerEvent OnLeftPlayerVariable;

    public UnityEvent OnConnectedEvent;
    public UnityEvent OnRoomListUpdated;
    public RoomInfoListEvent OnRoomListCompleteUpdated;


    public UnityEvent OnJoinedRoom;

    public UnityEvent OnLeftRoom;
    public UnityEvent OnLeftLobby;

    #region unity calls
    private void Awake()
    {
        lobby.PlayerEnteredEvent += EnteredPlayerEventHandler;
        lobby.ConnectedEvent += ConnectedEventHandler;
        lobby.RoomListUpdatedEvent += RoomListUpdatedEventHandler;
        lobby.JoinedRoomEvent += JoinedRoomEventHandler;
        lobby.PlayerLeftEvent += LeftRoomPlayerEventHandler;

        lobby.LeftRoomEvent += LeftdRoomEventHandler;
        lobby.LeftLobbyEvent += LeftdLobbyEventHandler;


        if (dontDestroyOnLoad)
            DontDestroyOnLoad(this.gameObject);

        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);

    }

    private void OnDestroy()
    {
        lobby.PlayerEnteredEvent -= EnteredPlayerEventHandler;
        lobby.ConnectedEvent -= ConnectedEventHandler;
        lobby.RoomListUpdatedEvent -= RoomListUpdatedEventHandler;
        lobby.JoinedRoomEvent -= JoinedRoomEventHandler;
        lobby.PlayerLeftEvent -= LeftRoomPlayerEventHandler;
        lobby.LeftRoomEvent -= LeftdRoomEventHandler;
        lobby.LeftLobbyEvent -= LeftdLobbyEventHandler;
    }

    private void Start()
    {
        if (AutologinAtStart != AutoLogin.None)
            Connect();
    }
    #endregion

    #region Public Calls

    public void Connect()
    {
        bool connectionResult = lobby.Connect((SystemInfo.deviceUniqueIdentifier + Random.Range(0, 9999999)).GetHashCode().ToString());
        if (connectionResult)
            Debug.Log("CONNECTED TO SERVER");
    }

    public void JoinLobby()
    {
        Debug.Log("joining lobby");
        lobby.JoinLobby();
    }

    public void DeletePlayer(Player player)
    {
        PhotonNetwork.DestroyPlayerObjects(player.ActorNumber, true);

    }

    public void RefreshRooms()
    {
        lobby.GetRooms();
        Debug.Log("Stanze:");
        for (int i = 0; i < lobby.Rooms.Count; i++)
            Debug.Log(lobby.Rooms[i].Name);


    }
    public void JoinFirstRoomOrCreate()
    {
        if (lobby.Rooms.Count == 0)
        {
            string name = "Room_" + System.DateTime.Now.Ticks;
            lobby.CreateRoom(true, true, name, MaxPlayers);
        }
        else
        {
            lobby.JoinRoom(lobby.Rooms[0].Name);
        }

    }
    #endregion



    #region Private Calls
    private void EnteredPlayerEventHandler(Player newPlayer)
    {

        OnEnteredPlayer?.Invoke();
        OnEnteredPlayerVariable?.Invoke(newPlayer);
        Debug.Log("player entered");
    }
    private void LeftRoomPlayerEventHandler(Player newPlayer)
    {
        Debug.Log("player left");
        OnLeftPlayer?.Invoke();
        OnLeftPlayerVariable?.Invoke(newPlayer);

    }

    private void ConnectedEventHandler()
    {
        if ((AutologinAtStart == AutoLogin.LobbyAndRoom) || (AutologinAtStart == AutoLogin.ConnectAndLobby))
            JoinLobby();

        Debug.Log("ConnectedEventHandler");
        OnConnectedEvent?.Invoke();
    }

    private void RoomListUpdatedEventHandler()
    {
        if ((AutologinAtStart == AutoLogin.LobbyAndRoom))
            JoinFirstRoomOrCreate();

        OnRoomListUpdated?.Invoke();
        OnRoomListCompleteUpdated?.Invoke(lobby.Rooms);
    }
  

    private void JoinedRoomEventHandler()
    {
        Debug.Log("JoinedRoomEventHandler");
        OnJoinedRoom?.Invoke();
    }


    private void LeftdRoomEventHandler()
    {
        Debug.Log("LeftdRoomEventHandler");
        OnLeftRoom?.Invoke();
    }

    private void LeftdLobbyEventHandler()
    {
        Debug.Log("LeftdLobbyEventHandler");
        OnLeftLobby?.Invoke();
    }
    #endregion
}