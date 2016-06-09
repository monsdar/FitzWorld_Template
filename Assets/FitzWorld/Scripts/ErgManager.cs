using UnityEngine;
using ProtoBuf;
using EasyErgsocket;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.Generic;
using System.Linq;

public class ErgManager : MonoBehaviour
{
    NetMQContext context = null;
    SubscriberSocket subSocket = null;
    
    //String: Id of the Erg
    //GameObject: The boat
    IDictionary<string, GameObject> boats = new Dictionary<string, GameObject>();
    string playerIndex = "";

    IList<Vector3> freeLanes = new List<Vector3>(); //these are the lanes for bots
    Vector3 playerLane = new Vector3(); //position of the lane where the player is on

    TrackManager trackManager = null;
    StatDisplayManager statDisplayManager = null;
    CameraManager cameraManager = null;

    public GameObject boatType;
    public int numLanes = 5;
    public int playerLaneIndex = 2;
    public float laneDistance = 4.0f;
    public string address = "tcp://127.0.0.1:21743";
    
    void Start ()
    {
        trackManager = GetComponent<TrackManager>();
        statDisplayManager = GetComponent<StatDisplayManager>();
        cameraManager = GetComponent<CameraManager>();

        initLanes();

        Debug.Log("Starting up NetMQ interface on " + address);
        context = NetMQContext.Create();
        subSocket = context.CreateSubscriberSocket();
        subSocket.Connect(address);
        subSocket.Subscribe("EasyErgsocket");
    }

    private void initLanes()
    {
        //populate the lane list according to the given values
        float startDistance = 0.0f - (laneDistance * playerLaneIndex);
        for (int index = 0; index < numLanes; index++)
        {
            Vector3 newLanePos = transform.position;
            newLanePos.z = startDistance + (laneDistance * index);
            if (index == playerLaneIndex)
            {
                playerLane = newLanePos;
            }
            else
            {
                freeLanes.Add(newLanePos);
            }
        }
    }

    void Update()
    {
        //Try to receive data and apply it to the boats
        IList<Erg> receivedBoats = ReceiveBoats();
        foreach (EasyErgsocket.Erg erg in receivedBoats)
        {
            UpdateErg(erg);
        }

        //do not do anything more if there's no player available
        if (!boats.ContainsKey(playerIndex))
        {
            return;
        }

        //if there's a player we need to modify the other boats
        Boat playerBoat = boats[playerIndex].GetComponentInChildren<Boat>();
        float playerDistance = playerBoat.Distance;

        if (statDisplayManager != null)
        {
            statDisplayManager.UpdatePosition(playerDistance);
        }

        foreach (var boat in boats)
        {
            //update the bots in relation to the players boat
            if (boat.Key != playerIndex)
            {
                Boat currentBoat = boat.Value.GetComponentInChildren<Boat>();
                currentBoat.AttachToBoat(playerDistance);
            }
        }
    }

    private IList<Erg> ReceiveBoats()
    {
        //try to receive something from the network... return all the ergs we get
        var message = new NetMQMessage();
        IList<EasyErgsocket.Erg> receivedBoats = new List<EasyErgsocket.Erg>();
        while (subSocket.TryReceiveMultipartMessage(System.TimeSpan.Zero, ref message))
        {
            foreach (var frame in message.Skip(1)) //the first frame is always just the envelope/topic... let's ignore it by using Linq
            {
                byte[] rawMessage = frame.Buffer;
                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(rawMessage))
                {
                    var givenErg = Serializer.Deserialize<EasyErgsocket.Erg>(memoryStream);
                    receivedBoats.Add(givenErg);
                }
            }
        }

        return receivedBoats;
    }

    private void UpdateErg(Erg givenErg)
    {
        //if the boat does not exist yet, add it
        if(!boats.ContainsKey(givenErg.ergId))
        {
            CreateBoat(givenErg);
        }

        Boat boat = boats[givenErg.ergId].GetComponentInChildren<Boat>();
        boat.UpdatePosition((float)givenErg.distance, (float)givenErg.exerciseTime);

        //update the stats on the track
        if (givenErg.playertype == EasyErgsocket.PlayerType.HUMAN)
        {
            if (statDisplayManager != null)
            {
                statDisplayManager.UpdateStats(givenErg);
            }
        }
    }

    private void CreateBoat(Erg givenErg)
    {
        Vector3 newPos;
        Quaternion newRot = new Quaternion();
        if (givenErg.playertype == EasyErgsocket.PlayerType.HUMAN)
        {
            newPos = playerLane;
            playerIndex = givenErg.ergId;
        }
        else
        {
            newPos = GetFreeBotLane();
        }

        Debug.Log("Created new Boat " + givenErg.ergId);
        boats[givenErg.ergId] = Instantiate(boatType, newPos, newRot) as GameObject;
        
        //if we just created the player boat we need to take care of positioning the camera, creating the track etc
        if (givenErg.playertype == EasyErgsocket.PlayerType.HUMAN)
        {
            if (cameraManager != null)
            {
                cameraManager.SetParent(boats[givenErg.ergId].transform);
            }

            if(trackManager != null)
            {
                trackManager.SetDistance((float)givenErg.distance);
            }
        }

        //update the boats name etc
        boats[givenErg.ergId].GetComponent<Boat>().BoatName = givenErg.name;
    }

    private Vector3 GetFreeBotLane()
    {
        if(freeLanes.Count >= 1)
        {
            Vector3 retValue = freeLanes[0];
            freeLanes.RemoveAt(0);
            return retValue;
        }

        return new Vector3();
    }

    void OnApplicationQuit()
    {
        Debug.Log("Shutting down...");
        subSocket.Close();
        context.Terminate();
        Debug.Log("Done...");
    }
}
