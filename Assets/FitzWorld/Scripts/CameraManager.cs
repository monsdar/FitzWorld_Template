using UnityEngine;
using System.Linq;

public class CameraManager : MonoBehaviour {

    public float TransitionFactor = 0.5f;

    Transform[] camPositions = null;
    int currentIndex = 0;
    int lastIndex = 0; //we need to know the last campos to know from which position we came from
    float startTime = 1.0f; //this is to store the time when the camera-transition has started

    Vector3 startingPos;
    Quaternion startingRot;

    //make this a Singleton to be called by other GameObjects
    static public CameraManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    public void SetParent(Transform parent)
    {
        Camera.main.transform.parent = parent;
        foreach(Transform camPos in camPositions)
        {
            camPos.parent = parent;
        }
    }

	// Use this for initialization
	void Start () {
        //get the camera positions from our children
        camPositions = gameObject.GetComponentsInChildren<Transform>();
        camPositions = camPositions.Skip(1).ToArray(); //remove the first element as this is the transform of the CameraManager-object itself...

        //init some variable that we need for cam switching
        currentIndex = 0;
        lastIndex = camPositions.Length-1;
        startTime = Time.time;

        //set the current transform of our camera to the first entry of cam positions
        Camera.main.transform.position = camPositions[currentIndex].position;
        Camera.main.transform.rotation = camPositions[currentIndex].rotation;
    }
	
	// Update is called once per frame
	void Update () {
        float currentTimeFactor = (Time.time - startTime) * TransitionFactor;
        //There are two possible ways of switching the cam:
        //  - Automatically whenever a cam-position is reach
        //  - On key press
        // Simply outcomment the method you would like to use:
        
        //if (currentTimeFactor >= 1.0f)    //Switch automatically
        if(Input.anyKeyDown)                //Switch on button press
        {
            lastIndex = currentIndex;
            currentIndex++;
            if (currentIndex >= camPositions.Length)
            {
                currentIndex = 0;
            }
            startTime = Time.time;
            Debug.Log("CamPos " + currentIndex + " reached. Switching to next position");
        }
        else
        {
            Camera.main.transform.position = Vector3.Lerp(camPositions[lastIndex].position, camPositions[currentIndex].position, currentTimeFactor);
            Camera.main.transform.rotation = Quaternion.Lerp(camPositions[lastIndex].rotation, camPositions[currentIndex].rotation, currentTimeFactor);
        }
    }
}
