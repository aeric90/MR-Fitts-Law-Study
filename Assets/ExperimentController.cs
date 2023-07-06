using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PROGRAM_STATUS
{
    TEST,
    START,
    PRACTICE,
    TRIAL,
    POST_TRIAL,
    END
}

public class ExperimentController : MonoBehaviour
{
    public static ExperimentController instance;

    private PROGRAM_STATUS currentStatus = PROGRAM_STATUS.TEST;

    private bool triggerDown = false;
    private bool buttonDown = false;

    private int conditionID = 0;
    public GameObject room;
    public GameObject avatar;

    public int[,] conditionSquare = {   {0, 1, 3, 2 },
                                        {1, 2, 0, 3 },
                                        {2, 3, 1, 0 },
                                        {3, 0, 2, 1 }};

    private int participantID = 1;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        SetCondition();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FittsVRController.fittsVRinstance.TargetSelected(Vector3.one);
        }

        if((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) && !triggerDown)
        {
            FittsVRController.fittsVRinstance.TargetSelected(Vector3.one);
            triggerDown = true;
        }

        if((OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger)) && triggerDown)
        {
            triggerDown = false;
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) && !buttonDown)
        {
            conditionID++;
            if (conditionID > 3) conditionID = 0;
            SetCondition();
            buttonDown = true;
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger) && buttonDown)
        {
            buttonDown = false;
        }
    }

    private void SetCondition()
    {
        switch (conditionID)
        {
            case 0:
            case 1:
                room.SetActive(false);
                break;
            case 2:
            case 3:
                room.SetActive(true);
                break;
        }
        switch (conditionID)
        {
            case 0:
            case 2:
                avatar.SetActive(false);
                break;
            case 1:
            case 3:
                avatar.SetActive(true);
                break;
        }
    }

    public void UIStart(string PID)
    {
        if (PID != "")
        {
            participantID = int.Parse(PID);
            currentStatus = PROGRAM_STATUS.PRACTICE;
            //SetNextVE();
        }
    }
}
