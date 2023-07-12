using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int currentTrial = -1;
    private int numberOfTrialsComplete = 0;
    private int currentTotalTargets = 0;
    private float currentAmplitude = 0.0f;
    private float currentTargetWidth = 1.0f;

    private List<GameObject> targets = new List<GameObject>();

    public int targetCount = 0;

    private int currentTargetIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        fittsVRinstance = this;
        //StartFitts();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Vector3 controllerPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            Vector2 controllerPos2d = new Vector2(controllerPos.x, controllerPos.y);
            Vector2 targetPos = new Vector2(targets[0].transform.position.x, targets[0].transform.position.y);

            float selectDistance = Vector2.Distance(controllerPos2d, targetPos);

            Debug.Log(selectDistance + ", " + currentTargetWidth);
        }
        */
    }

    public void StartFitts()
    {
        fittsRunning = true;
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

    public void TargetSelected(Vector3 selectionVector)
    {
        if (fittsRunning)
        {
            targetCount++;

            // Output:
            //  ID
            //  A
            //  W
            //  Time
            //  Selection V3
            //  Target V3

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
