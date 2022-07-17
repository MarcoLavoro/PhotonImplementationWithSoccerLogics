using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//class used to manage the events

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    public EventDataEvent OnStartGameEvent;
    public EventDataEvent OnChangeTeamEvent;
    public EventDataEvent OnGoalMade;

    #region public calls
    public void SendEventToAll(object[] content, EventsCodes eventCode)
    {
        byte code = (byte)eventCode;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(code, content, raiseEventOptions, SendOptions.SendReliable);
    }
    #endregion

    #region unity calls
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    #endregion

    #region private calls
    private void OnEvent(EventData photonEvent)
    {

        byte eventCode = photonEvent.Code;
        if (eventCode == (byte)EventsCodes.StartGameRequest)
        {
            Debug.Log("received start game");
            OnStartGameEvent.Invoke(photonEvent);
        }
        if (eventCode == (byte)EventsCodes.ChangeTeamEventCode)
        {
            OnChangeTeamEvent.Invoke(photonEvent);
        }
        if (eventCode == (byte)EventsCodes.GoalMade)
        {
            OnGoalMade.Invoke(photonEvent);
        }
    }
    #endregion
}
//al events codes availables
public enum EventsCodes
{
    ChangeTeamEventCode = 1,
    StartGameRequest = 2,
    GoalMade = 3
}