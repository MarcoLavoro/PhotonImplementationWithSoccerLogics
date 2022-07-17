using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    //use photonnetwork to load a specific level
    public void LoadLoevel(int id)
    {
        PhotonNetwork.LoadLevel(id);
    }
}
//categorizations of all the levels
public enum ScenesID
{
    ConnectingScene=0,
    GameScene =1
}
