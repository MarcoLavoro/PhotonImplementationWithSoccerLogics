using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickBall : MonoBehaviour
{
    //virtual kick based on position between player and the ball
    public float kickForce = 2;
    //only the masterclient give the "kick" action
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
            if (PhotonNetwork.IsMasterClient)
            {
                Vector3 direction = this.transform.position - collision.gameObject.transform.position;
                direction *= kickForce;
                //if the player hit the ball lets give to it a little push
                collision.gameObject.GetComponent<Rigidbody>().AddForce(direction, ForceMode.Impulse);

            }
    }
}
