using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


/**
* Script to transfer Position and Rotation of the "Cube Marker" to the Clients
*/
public class TargetCoordinatesScript : NetworkBehaviour {

    [SyncVar]
    public Vector3 trackerStartPosition;        
            
    [SyncVar]
    public float angle;

    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
