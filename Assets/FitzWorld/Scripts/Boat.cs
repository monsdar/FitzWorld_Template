using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

class MeasurePoint
{
    public float Distance { get; set; }
    public float ExerciseTime { get; set; }
    public float Timestamp { get; set; }
}

public class Boat : MonoBehaviour {

    public TextMesh nameText = null;
    public TextMesh distanceText = null;
    
    ICollection<MeasurePoint> measurements = new List<MeasurePoint>();
    const uint MAX_MEASUREMENTS = 30;
    float exerciseTime = 0.0f;
    
    private string boatName = "Boat";
    public string BoatName
    {
        get { return boatName; }
        set
        {
            boatName = value;

            //update nameText if there's one set
            if (nameText != null)
            {
                nameText.text = boatName;
            }
        }
    }

    public float Distance
    {
        get
        {
            //do not do anything if there are no measurements to calc with
            if (measurements.Count <= 1)
            {
                return 0.0f;
            }

            //calculate the distance over the last measured data
            float passedExerciseTime = measurements.Last().ExerciseTime - measurements.First().ExerciseTime;
            float passedDistance = measurements.Last().Distance - measurements.First().Distance;
            float currentVelocity = passedDistance / passedExerciseTime;

            float passedTime = Time.time - measurements.First().Timestamp;
            float deltaActualDistance = passedTime * currentVelocity;
            float newDistance = measurements.First().Distance + deltaActualDistance;
            
            return newDistance;
        }
    }
    
    public void AttachToBoat(float distance)
    {
        //update distanceText if there's one set
        if (distanceText != null)
        {
            float relativeDistance = transform.position.x - distance;
            distanceText.text = relativeDistance.ToString("0") + "m";

            Vector3 newDistPos = distanceText.transform.position;
            newDistPos.x = distance + 10.0f;
            distanceText.transform.position = newDistPos;
        }

        //update nameText if there's one set
        if (nameText != null)
        {
            Vector3 newNamePos = nameText.transform.position;
            newNamePos.x = distance - 8.0f;
            nameText.transform.position = newNamePos;
        }
    }

    public void UpdatePosition(float givenDistance, float givenExerciseTime)
    {
        //create a new measure point with the given data
        MeasurePoint newMeasurement = new MeasurePoint();
        newMeasurement.Distance = givenDistance;
        newMeasurement.ExerciseTime = givenExerciseTime;
        newMeasurement.Timestamp = Time.time;
        measurements.Add(newMeasurement);
        
        //keep only enough measurements to be able to calculate good values
        while (measurements.Count > MAX_MEASUREMENTS)
        {
            measurements.Remove(measurements.First());
        }

        if (distanceText != null)
        {
            //update the text-labels, these should display the most accurate data instead of calculated values
            distanceText.text = givenDistance.ToString("0") + "m";
        }
    }
	
	void Update ()
    {
        //update the boats position
        var newPos = transform.position;
        newPos.x = this.Distance;
        transform.position = newPos;
    }
}
