using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class TrackManager : MonoBehaviour
{
    //NOTE: This class has no pool of part instances - it seems as if Instantiating and Destroying is more performant than
    //      using setActive(true/false) or setScale(0.0/1.0) on a pool of instantiated parts... #lolUnity
    public List<GameObject> partTypes;
    public int maxFrontParts = 10;
    public int maxBackParts = 10;

    public GameObject bigGoalType;
    public GameObject smallGoalType;
    public float distancePerBigGoal = 100.0f;
    public float distancePerSmallGoal = 10.0f;

    System.Random rndGenerator = new System.Random();
    int lastRandomIndex = 999;

    List<GameObject> trackparts = new List<GameObject>();
    List<GameObject> bigGoals = new List<GameObject>();
    List<GameObject> smallGoals = new List<GameObject>();

    public void SetDistance(float distance)
    {
        //check how many parts are in front/behind the player
        int numFrontParts = 0;
        int numBackParts = 0;
        getFrontAndBackParts(distance, ref numFrontParts, ref numBackParts);

        //Create missing Front- and Backparts
        for (int index = 0; index < maxFrontParts - numFrontParts; ++index)
        {
            CreateFrontPart(distance);
            //TODO: Create goals
        }
        for (int index = 0; index < maxBackParts - numBackParts; ++index)
        {
            CreateBackPart(distance);
            //Do not create goals for parts in the back (that doesn't make sense, does it?)
        }

        //create the needed goals
        CreateGoals(bigGoalType, bigGoals, distancePerBigGoal);
        CreateGoals(smallGoalType, smallGoals, distancePerSmallGoal);

        //Remove Front- and Backparts if there are more than wanted
        for (int index = 0; index < numFrontParts - maxFrontParts; ++index)
        {
            GameObject lastPart = trackparts.Last();
            RemoveTrackPart(lastPart);
        }
        for (int index = 0; index < numBackParts - maxBackParts; ++index)
        {
            GameObject firstPart = trackparts.FirstOrDefault();
            RemoveTrackPart(firstPart);
        }
    }

    private void getFrontAndBackParts(float distance, ref int numFrontParts, ref int numBackParts)
    {
        foreach (GameObject part in trackparts)
        {
            if (part.transform.position.x < distance)
            {
                numBackParts++;
            }
            else
            {
                numFrontParts++;
            }
        }
    }

    private void RemoveTrackPart(GameObject part)
    {
        //clean up any goals behind the destroyed trackpart
        foreach (GameObject goal in bigGoals)
        {
            if (goal.transform.position.x <= part.transform.position.x)
            {
                Destroy(goal, 1.0f);
            }
        }
        foreach (GameObject goal in smallGoals)
        {
            if (goal.transform.position.x <= part.transform.position.x)
            {
                Destroy(goal, 1.0f);
            }
        }
        bigGoals.RemoveAll(item => (item.transform.position.x <= part.transform.position.x));
        smallGoals.RemoveAll(item => (item.transform.position.x <= part.transform.position.x));

        //destroy the trackpart itself
        trackparts.Remove(part);
        Destroy(part, 1.0f);

    }

    private void CreateBackPart(float startDistance)
    {
        //This method assumes that there are parts where we can dock on
        if(trackparts.Count <= 0)
        {
            return;
        }
        
        //if there already are some track parts we need to attach to them
        string frontDockName = "FrontDockingPoint";
        string backDockName = "BackDockingPoint";
        
        GameObject firstPart = trackparts.First();
        Transform frontDockingPoint = firstPart.GetComponentInChildren<Transform>().Find(frontDockName);
        Transform backDockingPoint = firstPart.GetComponentInChildren<Transform>().Find(backDockName);

        Transform dockingPoint = this.transform;
        dockingPoint.position = backDockingPoint.position - (frontDockingPoint.position - backDockingPoint.position);

        //create the new part
        GameObject newPart = Instantiate(GetRandomPartType(), dockingPoint.position, dockingPoint.rotation) as GameObject;
        trackparts.Insert(0, newPart);
    }

    private void CreateFrontPart(float givenDistance)
    {
        //if there's no track yet we need to use the given startdistance for our first trackpart
        Transform dockingPoint = this.transform;
        Vector3 newPos = dockingPoint.position;
        newPos.x = givenDistance;
        dockingPoint.position = newPos;

        //if there already are some track parts we need to attach to them
        if (trackparts.Count > 0)
        {
            string dockName = "FrontDockingPoint";
            GameObject lastPart = trackparts.Last();
            dockingPoint = lastPart.GetComponentInChildren<Transform>().Find(dockName);
        }

        //create the new part
        GameObject newPart = Instantiate(GetRandomPartType(), dockingPoint.position, dockingPoint.rotation) as GameObject;
        trackparts.Add(newPart);
    }

    private void CreateGoals(GameObject goalType, List<GameObject> goalList, float distancePerGoal)
    {
        if (goalType == null)
        {
            return;
        }
        
        //do nothing if there is no track yet
        if(trackparts.Count == 0)
        {
            return;
        }

        //if there is no goal yet try to create them from the first trackpart on... just go from the last known goal if there already are goals
        float lastGoalDistance = trackparts.First().transform.position.x;
        if (goalList.Count > 0)
        {
            lastGoalDistance = goalList.Last().transform.position.x;
        }

        //check the distance of the last created trackpart, create goals between the last goal and the last trackpart
        float lastTrackDistance = trackparts.Last().transform.position.x;
        while(lastGoalDistance < lastTrackDistance)
        {
            float nextDistance = lastGoalDistance + distancePerGoal;

            Transform dockingPoint = this.transform;
            Vector3 goalPosition = dockingPoint.position;
            goalPosition.x = nextDistance;
            dockingPoint.position = goalPosition;

            goalList.Add(Instantiate(goalType, dockingPoint.position, dockingPoint.rotation) as GameObject);
            lastGoalDistance = goalList.Last().transform.position.x;
        }
    }

    private GameObject GetRandomPartType()
    {
        //The while-loop ensures that we do not get the same indeces everytime
        int randomIndex = lastRandomIndex;
        while (lastRandomIndex == randomIndex)
        {
            randomIndex = rndGenerator.Next(partTypes.Count);
        }
        lastRandomIndex = randomIndex;
        return partTypes[randomIndex];
    }
}
