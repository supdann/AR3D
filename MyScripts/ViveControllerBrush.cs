using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ViveControllerBrush : NetworkBehaviour
{
    enum DRAW_SCRIPT_TYPE { STAR, RECTANGLE, TRIANGLE }
    enum MESH_OBJECT_COLOR { RED, YELLOW, TURQUOISE, BLUE, GREEN, ORANGE, PURPLE }

    //Reference to Tracker
    public GameObject wuerfel;

    public Vector3 currentPosition = new Vector3(0.0f, 0.0f, 0.0f);
    public Quaternion currentRotation = Quaternion.identity;

    //Initialization of the Htc Vive
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchPadButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;

    public bool triggerButtonPressed = false;
    public bool triggerButtonReleased = false;
    public bool touchPadButtonReleased = false;
    public bool menuButtonReleased = false;

    LinkedList<GameObject> meshHistory = new LinkedList<GameObject>();

    //Reference to draw mesh object
    public GameObject drawMeshObject;
    private GameObject currentDrawMeshObject;

    //Drawing controll
    private bool isDrawing = false;
    private DRAW_SCRIPT_TYPE currentDrawScript = DRAW_SCRIPT_TYPE.TRIANGLE;
    private DrawScript script;

    //Palette
    public GameObject editor;
    private bool editorIsActive = false;

    public GameObject cursorStar;
    public GameObject cursorTriangle;
    public GameObject cursorRectangle;

    public GameObject red;
    public GameObject yellow;
    public GameObject turquoise;
    public GameObject blue;
    public GameObject green;
    public GameObject purple;
    public GameObject orange;

    public GameObject brushObject;

    private Material currentMaterial;
    private MESH_OBJECT_COLOR currentColor;
    private bool selectColorAndFormMode = false;
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
        triggerButtonPressed = controller.GetPress(triggerButton);
        triggerButtonReleased = controller.GetPressUp(triggerButton);
        touchPadButtonReleased = controller.GetPressUp(touchPadButton);
        menuButtonReleased = controller.GetPressUp(menuButton);

        //Controller position and rotation for drawing
        currentPosition = controller.transform.pos; 
        currentRotation = controller.transform.rot;
        var pose = controller.GetPose();

        //Draw something, while the button is pressed
        if (triggerButtonPressed && !selectColorAndFormMode)
        {
            if(!isDrawing) { // Create a draw mesh object only the first frame when trigger pressed.

                currentDrawMeshObject = (GameObject) Instantiate(drawMeshObject, new Vector3(0, 0, 0), Quaternion.identity);
                NetworkServer.Spawn(currentDrawMeshObject);
                //networkScript.currentDrawMeshObject = currentDrawMeshObject;

                if (currentMaterial != null)
                {
                    currentDrawMeshObject.GetComponent<Renderer>().material = currentMaterial;
                    DrawMeshObjectMaterialController materialController = currentDrawMeshObject.GetComponent<DrawMeshObjectMaterialController>();

                    switch (currentColor)
                    {
                        case MESH_OBJECT_COLOR.BLUE:
                            materialController.RpcSetColor(1);
                            break;
                        case MESH_OBJECT_COLOR.RED:
                            materialController.RpcSetColor(2);
                            break;
                        case MESH_OBJECT_COLOR.YELLOW:
                            materialController.RpcSetColor(3);
                            break;
                        case MESH_OBJECT_COLOR.TURQUOISE:
                            materialController.RpcSetColor(4);
                            break;
                        case MESH_OBJECT_COLOR.ORANGE:
                            materialController.RpcSetColor(5);
                            break;
                        case MESH_OBJECT_COLOR.GREEN:
                            materialController.RpcSetColor(6);
                            break;
                        case MESH_OBJECT_COLOR.PURPLE:
                            materialController.RpcSetColor(7);
                            break;
                        default:
                            materialController.RpcSetColor(7);
                            break;
                    }
                }

                meshHistory.AddLast(currentDrawMeshObject);
                Network.Instantiate(currentDrawMeshObject, new Vector3(0, 0, 0), Quaternion.identity, 0);
                WuerfelController wuerfelController = wuerfel.GetComponent<WuerfelController>();

                DrawScript script = null;

                //Draw Script selection
                switch (currentDrawScript)
                {
                    case DRAW_SCRIPT_TYPE.TRIANGLE:
                        script = currentDrawMeshObject.GetComponent<TriangleDrawScript>();
                        break;
                    case DRAW_SCRIPT_TYPE.RECTANGLE:
                        script = currentDrawMeshObject.GetComponent<RectangleDrawScript>();
                        break;
                    case DRAW_SCRIPT_TYPE.STAR:
                        script = currentDrawMeshObject.GetComponent<StarDrawScript>();
                        break;
                    default:
                        script = currentDrawMeshObject.GetComponent<TriangleDrawScript>();
                        break;
                }
                script.initPatternPos = currentPosition; //CALLS HOOK initPattern()
                this.script = script;
                if (script != null && wuerfelController != null)
                {
                    Vector3 wuerfelPosition = wuerfelController.positionOfCubeInRoom;      //Marker position in Vive Coordinates
                    Quaternion wuerfelRotation = wuerfelController.rotationOfCubeInRoom;

                    //coordinates of wuerfel in room
                    script.cubePosition = wuerfelPosition;
                    //script.vertexRotation = wuerfelRotation;
                }
                isDrawing = true;
            }else {
                drawMesh();
            }
            
        }else if (triggerButtonReleased && isDrawing && !selectColorAndFormMode) {
            isDrawing = false;
            triggerButtonReleased = false;
        }

        if (touchPadButtonReleased) 
        {
            if (meshHistory.Last != null)
            {
                GameObject lastMesh = meshHistory.Last.Value;
                meshHistory.RemoveLast(); // remove the linked list entry
                Network.Destroy(lastMesh);
                Destroy(lastMesh);  //Delete latest drawed object
            }
        }

        if (menuButtonReleased)
        {
            if (!editorIsActive)
            {
                editor.SetActive(true);
                editorIsActive = true;
                selectColorAndFormMode = true;
            }
            else
            {
                editor.SetActive(false);
                editorIsActive = false;
                selectColorAndFormMode = false;
            }
        }
    }
    
    void drawMesh()
    {
        if (script != null)
        {
            //coordinates for drawing
            script.newPosition = currentPosition;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Material localSelectedMaterial = null;

        print("Started colliding with object " + gameObject.name + " and trigger object " + other.name);

        if (other.name.ToLower() == orange.name.ToLower())
        {
            localSelectedMaterial = orange.GetComponent<Renderer>().sharedMaterial;
            currentColor = MESH_OBJECT_COLOR.ORANGE;
        }
        else
        if (other.name.ToLower() == turquoise.name.ToLower())
        {
            localSelectedMaterial = turquoise.GetComponent<Renderer>().sharedMaterial;
            currentColor = MESH_OBJECT_COLOR.TURQUOISE;
        }
        else
        if (other.name.ToLower() == green.name.ToLower())
        {
            localSelectedMaterial = green.GetComponent<Renderer>().sharedMaterial;
            currentColor = MESH_OBJECT_COLOR.GREEN;
        }
        else
        if (other.name.ToLower() == purple.name.ToLower())
        {
            localSelectedMaterial = purple.GetComponent<Renderer>().sharedMaterial;
            currentColor = MESH_OBJECT_COLOR.PURPLE;
        }
        else
        if (other.name.ToLower() == yellow.name.ToLower())
        {
            localSelectedMaterial = yellow.GetComponent<Renderer>().sharedMaterial;
            currentColor = MESH_OBJECT_COLOR.YELLOW;
        }
        else
        if (other.name.ToLower() == red.name.ToLower())
        {
            localSelectedMaterial = red.GetComponent<Renderer>().sharedMaterial;
            currentColor = MESH_OBJECT_COLOR.RED;
        }
        else
        if (other.name.ToLower() == blue.name.ToLower())
        {
            localSelectedMaterial = blue.GetComponent<Renderer>().sharedMaterial;
            currentColor = MESH_OBJECT_COLOR.BLUE;
        }

        brushObject.GetComponent<Renderer>().material = localSelectedMaterial;
        currentMaterial = localSelectedMaterial;

        if (other.name == cursorStar.name)
        {
            currentDrawScript = DRAW_SCRIPT_TYPE.STAR;
        }

        if (other.name == cursorTriangle.name)
        {
            currentDrawScript = DRAW_SCRIPT_TYPE.TRIANGLE;
        }

        if (other.name == cursorRectangle.name)
        {
            currentDrawScript = DRAW_SCRIPT_TYPE.RECTANGLE;
        }

    }
}
