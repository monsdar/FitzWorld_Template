using UnityEngine;
using System.Collections;

public class CollNotifierTrackpart : MonoBehaviour
{
    public TrackManager trackManager = null;
    bool hasCollided = false;
    void OnCollisionEnter()
    {
        if(!hasCollided)
        {
            if(trackManager != null)
            {
                trackManager.NotifyCollision(gameObject.GetInstanceID());
            }
            hasCollided = true;
        }
    }
}
