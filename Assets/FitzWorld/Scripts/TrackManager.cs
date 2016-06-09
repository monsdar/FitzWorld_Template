using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TrackManager : MonoBehaviour
{
    public GameObject partType;
    public GameObject goalType;
    public int trackPartsPerGoal = 1;
    public int maxParts = 20;

    IList<GameObject> trackparts = new List<GameObject>();
    IList<GameObject> goals = new List<GameObject>();

    //This is called whenever a collision with a track part has happened
    public void NotifyCollision(int senderId)
    {
        //put a new part to the back of the track
        CreateBackPart();
        CleanParts(maxParts);
    }
    
    public void SetDistance(float distance)
    {
        Vector3 trackPos = transform.position;
        trackPos.x = distance - (distance % 100.0f);
        transform.position = trackPos;

        //this cleans any previously created tracks
        CleanParts(0);

        //create some track parts on front of the player and some in the back... this will be the track we're working with
        for (int index = 0; index < maxParts / 2; index++)
        {
            CreateBackPart();
        }
        for (int index = 0; index < maxParts / 2; index++)
        {
            CreateFrontPart();
        }
        CleanParts(maxParts);
    }

    private void CleanParts(int numMaxParts)
    {
        //just maintain a certain number of parts
        while (trackparts.Count > numMaxParts)
        {
            GameObject firstPart = trackparts.FirstOrDefault();
            trackparts.Remove(firstPart);
            Destroy(firstPart, 1.0f);
        }

        //clean up all goals that reach back longer than the track is long
        if(trackparts.Count > 0)
        {
            float trackOrigin = trackparts.FirstOrDefault().transform.position.x;
            foreach (GameObject goal in goals.Reverse()) //iterate reverse, this enables us to remove during iteration
            {
                if (goal.transform.position.x < trackOrigin)
                {
                    goals.Remove(goal);
                    Destroy(goal, 1.0f);
                }
            }
        }
    }

    private void CreateBackPart()
    {
        //start from the object that we're attached on if there's no track yet
        Transform dockingPoint = transform;

        //if there already are some track parts we need to attach to them
        if (trackparts.Count > 0)
        {
            string dockName = "BackDockingPoint";
            GameObject lastPart = trackparts.Last();
            dockingPoint = lastPart.GetComponentInChildren<Transform>().Find(dockName);
        }

        //the new track part needs a reference to the trackmanager
        GameObject newPart = Instantiate(partType, dockingPoint.position, dockingPoint.rotation) as GameObject;
        CollNotifierTrackpart collNotifier = newPart.GetComponentInChildren<CollNotifierTrackpart>();
        collNotifier.trackManager = this;
        trackparts.Add(newPart);

        //check if we need to create a goal too
        //TODO: Create goals every x meters, not every x parts
        if(trackparts.Count % trackPartsPerGoal == 0)
        {
            if(goalType != null)
            {
                goals.Add(Instantiate(goalType, dockingPoint.position, dockingPoint.rotation) as GameObject);
            }
        }
    }

    private void CreateFrontPart()
    {
        //start from the object that we're attached on if there's no track yet
        Transform dockingPoint = transform;

        //if there already are some track parts we need to attach to them
        string frontDockName = "FrontDockingPoint";
        string backDockName = "BackDockingPoint";

        GameObject firstPart = trackparts.FirstOrDefault();
        Transform frontDockingPoint = firstPart.GetComponentInChildren<Transform>().Find(frontDockName);
        Transform backDockingPoint = firstPart.GetComponentInChildren<Transform>().Find(backDockName);
        dockingPoint.position = frontDockingPoint.position - (backDockingPoint.position - frontDockingPoint.position);

        //the new track part needs a reference to the trackmanager
        GameObject newPart = Instantiate(partType, dockingPoint.position, dockingPoint.rotation) as GameObject;
        CollNotifierTrackpart collNotifier = newPart.GetComponentInChildren<CollNotifierTrackpart>();
        collNotifier.trackManager = this;
        trackparts.Insert(0,  newPart);
    }
}
