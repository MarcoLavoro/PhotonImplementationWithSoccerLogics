using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;

//a custom lobby, to personalize the behavour if needed
public class LobbyCustom : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public event Action<Player> PlayerEnteredEvent;

    public event Action ConnectedEvent;

    public event Action RoomListUpdatedEvent;

    public event Action JoinedLobbyEvent;

    public event Action JoinedRoomEvent;

    public event Action LeftRoomEvent;

    public event Action LeftLobbyEvent;

    public event Action<Player> PlayerLeftEvent;

    public List<RoomInfo> Rooms { get; private set; }

    public Room CurrentRoom => PhotonNetwork.CurrentRoom;

    public bool Connect(string userId)
    {
        if (PhotonNetwork.IsConnected)
            return true;

        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = userId;
        return PhotonNetwork.ConnectUsingSettings();
    }

    public void Disconnect()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        PhotonNetwork.Disconnect();
    }

    public bool JoinLobby()
    {
        if (PhotonNetwork.InLobby)
            return true;

        return PhotonNetwork.JoinLobby();
    }

    public void GetRooms()
    {
        PhotonNetwork.GetCustomRoomList(TypedLobby.Default, string.Empty);
    }

    public void CreateRoom(bool open, bool visible, string roomName, byte maxPlayers = 0)
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = maxPlayers,
            IsOpen = open,
            IsVisible = visible,
            PublishUserId = false,
            CleanupCacheOnLeave = true,
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinRoom(string roomName = null)
    {
        if (PhotonNetwork.InRoom)
            return;

        LeaveLobby();

        if (string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public void LeaveRoom()
    {
        if (!PhotonNetwork.InRoom)
            return;

        PhotonNetwork.LeaveRoom();
    }

    public void LeaveLobby()
    {
        if (!PhotonNetwork.InLobby)
            return;

        PhotonNetwork.LeaveLobby();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        Rooms = roomList;

        RoomListUpdatedEvent?.Invoke();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        JoinedRoomEvent?.Invoke();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        JoinedLobbyEvent?.Invoke();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        ConnectedEvent?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        PlayerEnteredEvent?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        PlayerLeftEvent?.Invoke(otherPlayer);

    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        LeftRoomEvent?.Invoke();
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();

        LeftLobbyEvent?.Invoke();
    }


}