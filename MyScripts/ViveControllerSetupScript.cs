using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ViveControllerSetupScript : MonoBehaviour
{
    public Vector3 currentPosition = new Vector3(0.0f, 0.0f, 0.0f);
    public Quaternion currentRotation = Quaternion.identity;

    private bool tracketWasSet = false;
    private bool startPositionWasSet = false;

    public Vector3 firstPressPosition;
    public Vector3 secondPressPosition;
    public float angle;

    //Initialization of the Htc Vive
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    public bool triggerButtonReleased = false;

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        //change booleans with button pressing
        triggerButtonReleased = controller.GetPressUp(triggerButton);

        //Controller position and rotation for drawing
        currentPosition = controller.transform.pos; 
        currentRotation = controller.transform.rot;

        if (triggerButtonReleased)
        {
            print("Trigger pressed down");
            if (tracketWasSet == false)
            {
                if (!startPositionWasSet && triggerButtonReleased)
                {
                    firstPressPosition = currentPosition;

                    startPositionWasSet = true;
                }
                else if (startPositionWasSet && triggerButtonReleased)
                {
                    //Position number 2 (bottom center of cube) => Position of the cube
                    secondPressPosition = currentPosition;

                    //Angle calculation
                    //We take out the y-axis because our cube is always even on the x- and z-axis
                    Vector3 newFirstPressPosition = new Vector3(firstPressPosition.x, 0, firstPressPosition.z);
                    Vector3 newSecondPressPosition = new Vector3(secondPressPosition.x, 0, secondPressPosition.z);
                    Vector3 distanceVector = newSecondPressPosition - newFirstPressPosition;
                    angle = Vector3.Angle(new Vector3(0, 0, 1), distanceVector);

                    GameObject TargetCoordinatesManager = GameObject.Find("TargetCoordinatesManager");
                    TargetCoordinatesScript script = TargetCoordinatesManager.GetComponent<TargetCoordinatesScript>();
                    script.trackerStartPosition = secondPressPosition;
                    script.angle = angle;

                    //To load scene on server only do: SceneManager.LoadScene("Main"); otherwise do: 
                    NetworkManager.singleton.ServerChangeScene("Main");

                    tracketWasSet = true;
                }
            }
        }
    }
}
