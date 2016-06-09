using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StatDisplayManager : MonoBehaviour
{
    public TextMesh Slot1 = null;
    public TextMesh Slot2 = null;
    public TextMesh Slot3 = null;
    public TextMesh Slot4 = null;
    public TextMesh Slot5 = null;
    
    public void UpdateStats(EasyErgsocket.Erg givenData)
    {
        if( Slot1 != null &&
            Slot2 != null &&
            Slot3 != null &&
            Slot4 != null &&
            Slot5 != null)
        {
            Slot1.text = givenData.distance.ToString("0.00") + "m";
            Slot2.text = getPaceStr(givenData.paceInSecs);
            Slot3.text = getPaceStr(givenData.paceInSecs);
            Slot4.text = givenData.cadence.ToString();
            Slot5.text = getTimeStr(givenData.exerciseTime);
        }
    }

    public void UpdatePosition(float distance)
    {
        var newPos = transform.position;
        newPos.x = distance;
        transform.position = newPos;
    }

    private string getTimeStr(double seconds)
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);
        string answer = string.Format("{0:D1}:{1:D2}:{2:D2}",
                        t.Hours,
                        t.Minutes,
                        t.Seconds);
        return answer;
    }

    private string getPaceStr(double seconds)
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);
        string answer = string.Format("{0:D2}:{1:D2}.{2:D1}",
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
        return answer;
    }
}
