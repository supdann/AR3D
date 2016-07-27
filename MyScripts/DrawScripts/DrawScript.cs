using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public abstract class DrawScript: NetworkBehaviour {

    //Synchronize cube position with all Clients
    [SyncVar]
    public Vector3 cubePosition;

    [SyncVar(hook = "initPattern")]
    public Vector3 initPatternPos;

    //Call these methods, if the variables get updatet
    [SyncVar(hook = "addNewVertexPosition")]
    public Vector3 newPosition;

    public abstract void initPattern(Vector3 initPos);
    public abstract void addNewVertexPosition(Vector3 newPosition);
    public abstract void DefineTriangles();



}
