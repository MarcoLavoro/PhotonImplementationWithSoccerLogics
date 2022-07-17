using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//all events that could be helpful
[Serializable]
public class StringEvent : UnityEvent<string> { }

[Serializable]
public class BoolEvent : UnityEvent<bool> { }

[Serializable]
public class PlayerEvent : UnityEvent<Player> { }

[Serializable]
public class RoomInfoListEvent : UnityEvent<List<RoomInfo>> { }

[Serializable]
public class EventDataEvent : UnityEvent<EventData> { }