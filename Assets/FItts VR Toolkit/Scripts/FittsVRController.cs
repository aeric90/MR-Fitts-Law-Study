using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class FittsCondition
{
    public int numOfTargets = 0;
    public float amplitude = 0.0f;
    public float width = 0.0f;

}

public class FittsVRController : MonoBehaviour
{
    public static FittsVRController fittsVRinstance;

    public int participantID = 0;
    public FittsCondition[] trialConditions;
    public FittsCondition[] practiceConditions;

    public int[,] conditionSquare = {   {0, 1, 3, 4, 5, 2, 0, 1, 3, 4, 5, 2 },
                                        {1, 4, 0, 2, 3, 5, 1, 4, 0, 2, 3, 5 },
                                        {4, 2, 1, 5, 0, 3, 4, 2, 1, 5, 0, 3 },
                                        {2, 5, 4, 3, 1, 0, 2, 5, 4, 3, 1, 0 },
                                        {5, 3, 2, 0, 4, 1, 5, 3, 2, 0, 4, 1 },
                                        {3, 0, 5, 1, 2, 4, 3, 0, 5, 1, 2, 4 }};

    public int inputTotalTargets = 0;
    public float inputAmplitude = 0.0f;
    public float inputTargetWidth = 1.0f;

    public GameObject targetPrefab;
    public Material targetBasicMaterial;
    public Material targetActiveMaterial;
    public Material targetInactiveMaterial;

    public GameObject targetContainer;

    private bool fittsRunning = false;
    private bool practiceState = false;
    private bool practiceComplete = false;
    private bool trialComplete = false;
    private int expTrailID = 0;
    private int currentTrial = -1;
    private int numberOfTrialsComplete = 0;
    private int currentTotalTargets = 0;
    private float currentAmplitude = 0.0f;
    private float currentTargetWidth = 1.0f;
    private float lastTargetTime = 0.0f;

    private List<GameObject> targets = new List<GameObject>();

    public int targetCount = 0;

    private int currentTargetIndex = 0;

    private StreamWriter output;

    // Start is called before the first frame update
    void Start()
    {
        fittsVRinstance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartFitts(int trialID)
    {
        expTrailID = trialID;
        fittsRunning = true;
        numberOfTrialsComplete = 0;
        NextTrial();
    }

    private void ResetTargets()
    {
        DeleteTargets();

        for(float i = 0.0f; i < currentTotalTargets; i++)
        {
            float x = targetContainer.transform.position.x + (currentAmplitude / 2.0f) * Mathf.Cos((Mathf.PI * 2) * (i / currentTotalTargets));
            float y = targetContainer.transform.position.y + (currentAmplitude / 2.0f) * Mathf.Sin((Mathf.PI * 2) * (i / currentTotalTargets));
            GameObject newTarget = Instantiate(targetPrefab, new Vector3(x, y, targetContainer.transform.position.z), targetPrefab.transform.rotation, targetContainer.transform);
            newTarget.transform.localScale = new Vector3(currentTargetWidth, 0.01f, currentTargetWidth); 
            targets.Add(newTarget);
        }
        targetCount = 0;
        SetNextActiveTarget();
    }

    private void DeleteTargets()
    {
        foreach(GameObject target in targets) DestroyImmediate(target);
        targets.Clear();
    }

    private void SetNextActiveTarget()
    {
        if (targetCount > 0) {
            if (targetCount == 1)
            {
                targets[currentTargetIndex].gameObject.GetComponent<Renderer>().material = targetBasicMaterial;
            }
            else
            {
                targets[currentTargetIndex].gameObject.GetComponent<Renderer>().material = targetInactiveMaterial;
            }

            if (targetCount < currentTotalTargets)
            {
                int halfWay = (currentTotalTargets + 1) / 2;
                currentTargetIndex = (currentTargetIndex + halfWay) % currentTotalTargets;
            } else
            {
                currentTargetIndex = 0;
            }
        }

        targets[currentTargetIndex].gameObject.GetComponent<Renderer>().material = targetActiveMaterial;
    }

    public void StartTrials()
    {
        output = new StreamWriter(Application.persistentDataPath + "/FittsVR-Results-" + DateTime.Now.ToString("ddMMyy-MMss-") + participantID + ".csv");
        output.WriteLine("TID,PID,#,A,W,ID,T,sX,sY,sZ,tX,tY,tZ,dX,dY,dZ,dP");
    }

    public void EndTrials()
    {
        Debug.Log("Closing File");
        output.Close();
    }

    public void TargetSelected(Vector3 selectionVector)
    {
        if (fittsRunning)
        {
            GetComponent<AudioSource>().Play();

            if (!practiceState && targetCount > 0)
            {
                string outputLine = "";

                outputLine += expTrailID + ",";
                outputLine += participantID + ",";
                outputLine += targetCount + ",";
                outputLine += currentAmplitude + ",";
                outputLine += currentTargetWidth + ",";
                outputLine += Math.Log((currentAmplitude / currentTargetWidth) + 1, 2) + ",";
                outputLine += Time.time - lastTargetTime + ",";
                outputLine += selectionVector.x + ",";
                outputLine += selectionVector.y + ",";
                outputLine += selectionVector.z + ",";
                outputLine += targets[currentTargetIndex].transform.position.x + ",";
                outputLine += targets[currentTargetIndex].transform.position.y + ",";
                outputLine += targets[currentTargetIndex].transform.position.z + ",";

                float xDelta = Math.Abs(targets[currentTargetIndex].transform.position.x - selectionVector.x);
                float yDelta = Math.Abs(targets[currentTargetIndex].transform.position.y - selectionVector.y);
                float zDelta = Math.Abs(targets[currentTargetIndex].transform.position.z - selectionVector.z);

                outputLine += xDelta + ",";
                outputLine += yDelta + ",";
                outputLine += zDelta + ",";
                outputLine += Math.Sqrt(Math.Pow(xDelta, 2) + Math.Pow(yDelta, 2));

                output.WriteLine(outputLine);
            }

            lastTargetTime = Time.time;
            targetCount++;

            if (targetCount > currentTotalTargets)
            {
                numberOfTrialsComplete++;
                if (numberOfTrialsComplete < 12)
                {
                    NextTrial();
                } else
                {
                    fittsRunning = false;
                    trialComplete = true;
                    DeleteTargets();
                }
            }
            else
            {
                SetNextActiveTarget();
            }
        }
    }

    private void NextTrial()
    {
        FittsCondition newCondition = new FittsCondition();

        if(practiceState) {
            currentTrial++;
            if (currentTrial >= practiceConditions.Length)
            {
                practiceComplete = true;
                currentTrial = 0;
            }
            newCondition = practiceConditions[currentTrial];
        } else {
            currentTrial = conditionSquare[participantID % 6, numberOfTrialsComplete];
            newCondition = trialConditions[currentTrial];
        }

        targetCount = 0;
        trialComplete = false;
        currentTotalTargets = newCondition.numOfTargets;
        currentAmplitude = newCondition.amplitude;
        currentTargetWidth = newCondition.width;

        ResetTargets();
    }
    public void SetPractice(bool status)
    {
        practiceState = status;
    }

    public void SetPID(int PID)
    {
        participantID = PID;
    }

    public bool GetPracticeComplete()
    {
        return practiceComplete;
    }

    public bool GetTrialComplete()
    {
        return trialComplete;
    }
}
