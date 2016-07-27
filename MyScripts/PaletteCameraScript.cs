using UnityEngine;
using System.Collections;

public class PaletteCameraScript : MonoBehaviour {

    public GameObject camera;

    public float xOffset;
    public float yOffset;
    public float zOffset;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (camera != null)
        {
            transform.localPosition = new Vector3(xOffset, yOffset, zOffset);
            //transform.localRotation = camera.transform.rotation;
        }
	}
}
