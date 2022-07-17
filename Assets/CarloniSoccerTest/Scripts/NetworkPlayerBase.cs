using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetworkPlayerBase : MonoBehaviour
{
    //this scirpt is used to let execute update or in another script, ONLY if the condition photonView.IsMine is true
    [SerializeField]
    public UnityEvent UpdateIfMine;

    [SerializeField]
    public BoolEvent StartIfMine;


    private PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
            StartIfMine?.Invoke(photonView.IsMine);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (photonView.IsMine)
            UpdateIfMine?.Invoke();
    }
}
