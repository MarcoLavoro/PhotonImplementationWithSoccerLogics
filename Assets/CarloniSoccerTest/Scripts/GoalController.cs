using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    [HideInInspector]
    public bool IsPlayer1;
    PhotonView photonView;
    private void Awake()
    {
        //get photonview
        photonView = GetComponent<PhotonView>();
    }
    private void OnTriggerEnter(Collider other)
    {
        //if the ball enter and is mine trigger the goal event
        if (photonView.IsMine)
            if (other.gameObject.tag == "Ball")
                SendGoalEvent(IsPlayer1);

    }
    private void SendGoalEvent(bool isPlayer1)
    {
        object[] content = new object[] { isPlayer1 }; // Array contains the target position and the IDs of the selected units
        EventManager.Instance.SendEventToAll(content, EventsCodes.GoalMade);
    }
}
