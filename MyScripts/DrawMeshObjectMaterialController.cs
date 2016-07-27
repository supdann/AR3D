using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class DrawMeshObjectMaterialController : NetworkBehaviour {

    public GameObject red;
    public GameObject yellow;
    public GameObject turquoise;
    public GameObject blue;
    public GameObject green;
    public GameObject purple;
    public GameObject orange;

    [ClientRpc]
    public void RpcSetColor(int color)
    {
        switch (color)
        {
            case 1:
                GetComponent<Renderer>().material = blue.GetComponent<Renderer>().material;
                break;
            case 2:
                GetComponent<Renderer>().material = red.GetComponent<Renderer>().material;
                break;
            case 3:
                GetComponent<Renderer>().material = yellow.GetComponent<Renderer>().material;
                break;
            case 4:
                GetComponent<Renderer>().material = turquoise.GetComponent<Renderer>().material;
                break;
            case 5:
                GetComponent<Renderer>().material = orange.GetComponent<Renderer>().material;
                break;
            case 6:
                GetComponent<Renderer>().material = green.GetComponent<Renderer>().material;
                break;
            case 7:
                GetComponent<Renderer>().material = purple.GetComponent<Renderer>().material;
                break;
            default:
                GetComponent<Renderer>().material = purple.GetComponent<Renderer>().material;
                break;
        }
        
    }

}
