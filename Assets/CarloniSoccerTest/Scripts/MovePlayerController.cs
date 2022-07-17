using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerController : MonoBehaviour
{
    public Rigidbody myBody;
    public float moveSpeed = 50f;
    // Start is called before the first frame update
    void Start()
    {
        myBody=GetComponent<Rigidbody>();
    }
    //action to move the character (will be called by another scripts, with a separated method we can test this function also without a multiplayer environment
    public void MoveCharacter()
    {
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir *= moveSpeed;
        Vector3 destination = new Vector3(targetDir.x,0,targetDir.y);
        myBody.AddForce(destination);
        
    }
}
