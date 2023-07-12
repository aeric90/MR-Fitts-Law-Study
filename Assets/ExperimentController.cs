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
    private int trialNumber = 0;
    private string hand = "";

    public GameObject PIDUI;
    public GameObject HandUI;
    public GameObject room;
    public GameObject avatar;
    public GameObject fittsController;
    public GameObject nextUI;

    public int[,] conditionSquare = {{0, 1, 3, 2 },
                                     {1, 2, 0, 3 },
                                     {2, 3, 1, 0 },
                                     {3, 0, 2, 1 }};

    private int participantID = 1;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //SetCondition();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FittsVRController.fittsVRinstance.TargetSelected(Vector3.one);
        }

        if (currentStatus == PROGRAM_STATUS.PRACTICE || currentStatus == PROGRAM_STATUS.TRIAL)
        {
            if ((OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) && !triggerDown)
            {
                FittsVRController.fittsVRinstance.TargetSelected(Vector3.one);
                triggerDown = true;
            }

            if ((OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger)) && triggerDown)
            {
                triggerDown = false;
            }
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

        switch(currentStatus)
        {
            case PROGRAM_STATUS.TEST:
                break;
            case PROGRAM_STATUS.START:
                break;
            case PROGRAM_STATUS.PRACTICE:
                if(FittsVRController.fittsVRinstance.GetPracticeComplete()) nextUI.SetActive(true);
                break;
            case PROGRAM_STATUS.TRIAL:
                if (FittsVRController.fittsVRinstance.GetTrialComplete()) nextUI.SetActive(true);
                NewStatus(PROGRAM_STATUS.POST_TRIAL);
                break;
            case PROGRAM_STATUS.POST_TRIAL:
                break;
            case PROGRAM_STATUS.END:
                break;
        }
    }

    private void GetNextCondition()
    {


    }

    private void SetCondition()
    {
        conditionID = conditionSquare[participantID % 4, trialNumber];

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
        hand = HandUIController.instance.hand;

        if (PID != "" && hand != "")
        {
            participantID = int.Parse(PID);
            NewStatus(PROGRAM_STATUS.PRACTICE);
        }
    }

    public void NextCondition()
    {
        trialNumber++;
        if (trialNumber <= 4)
        {
            currentStatus = PROGRAM_STATUS.END;
        }
    }

    public void NextButton()
    {
        switch (currentStatus)
        {
            case PROGRAM_STATUS.PRACTICE:
                FittsVRController.fittsVRinstance.SetPractice(false);
                NewStatus(PROGRAM_STATUS.TRIAL);
                break;
            case PROGRAM_STATUS.POST_TRIAL:
                NextCondition();
                NewStatus(PROGRAM_STATUS.TRIAL);
                break;
        }
    }

    private void NewStatus(PROGRAM_STATUS newStatus)
    {
        switch (newStatus)
        {
            case PROGRAM_STATUS.TEST:
                break;
            case PROGRAM_STATUS.START:
                break;
            case PROGRAM_STATUS.PRACTICE:
                PIDUI.SetActive(false);
                HandUI.SetActive(false);
                FittsVRController.fittsVRinstance.SetPractice(true);
                FittsVRController.fittsVRinstance.SetPID(participantID);
                FittsVRController.fittsVRinstance.StartFitts();
                currentStatus = PROGRAM_STATUS.PRACTICE;
                break;
            case PROGRAM_STATUS.TRIAL:
                nextUI.SetActive(false);
                FittsVRController.fittsVRinstance.StartFitts();
                currentStatus = PROGRAM_STATUS.TRIAL;
                SetCondition();
                break;
            case PROGRAM_STATUS.POST_TRIAL:
                currentStatus = PROGRAM_STATUS.POST_TRIAL;
                break;
        }
    }

    private void CheckTrialStatus(int targetCount)
    {
        if(targetCount == FittsVRController.fittsVRinstance.targetCount)
        {

        }
    }
}
