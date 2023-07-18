using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FittsVRControllerTracker : MonoBehaviour
{
    public static FittsVRControllerTracker fittsVRControllerTracker;

    public float timeToRecord = 0.25f;
    private float lastCheck;

    private int participantID = -1;
    private int expTrailID = -1;
    private GameObject trackedObject = null;
    private bool trackerOn = false;

    private StreamWriter output;

    // Start is called before the first frame update
    void Start()
    {
        fittsVRControllerTracker = this;
        lastCheck = Time.time;
    }

    public void SetPID(int PID)
    {
        participantID = PID;
    }

    public void SetTrackedObject(GameObject trackedObject)
    {
        this.trackedObject = trackedObject;
    }

    public void SetExpTrial(int trialID)
    {
        expTrailID = trialID;  
    }

    public void SetTrackerOn(bool status)
    {
        trackerOn = status;

        if (status)
        {
            output = new StreamWriter(Application.persistentDataPath + "/FittsVR-Tracking-" + DateTime.Now.ToString("ddMMyy-MMss-") + participantID + ".csv");
            output.WriteLine("T,TID,PID,X,Y,Z");
        }
        else if (!status)
        {
            output.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(trackerOn && trackedObject != null)
        {
            if(lastCheck - Time.deltaTime >= timeToRecord)
            {
                string outputString = "";
                outputString += Time.time + ",";
                outputString += expTrailID + ",";
                outputString += participantID + ",";
                outputString += trackedObject.transform.position.x + ",";
                outputString += trackedObject.transform.position.y + ",";
                outputString += trackedObject.transform.position.z;

                output.WriteLine(outputString);
            }

            lastCheck = Time.time;
        }
    }
}
