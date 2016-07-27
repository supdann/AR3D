using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class WuerfelController : NetworkBehaviour
{
    public Vector3 positionOfCubeInRoom;
    public Quaternion rotationOfCubeInRoom;

    // Use this for initialization
    void Start()
    {
            GameObject TargetCoordinatesManager = GameObject.Find("TargetCoordinatesManager");
            TargetCoordinatesScript script = TargetCoordinatesManager.GetComponent<TargetCoordinatesScript>();

            positionOfCubeInRoom = script.trackerStartPosition;
            rotationOfCubeInRoom = Quaternion.Euler(new Vector3(0, script.angle, 0));
            
            //Server
            if (isServer)
            {
            //Translate the cube to the correct Position for the person with the VR headset
                transform.position = positionOfCubeInRoom;
                transform.rotation = rotationOfCubeInRoom;
            }

            //Clients
            else
            {
                transform.position = new Vector3(0, 0, 0);
                transform.rotation = Quaternion.identity;
        }
    }
}
