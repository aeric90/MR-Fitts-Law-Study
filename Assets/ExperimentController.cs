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

    public PROGRAM_STATUS currentStatus = PROGRAM_STATUS.TEST;

    private bool triggerDown = false;

    private int conditionID = 0;
    private int trialNumber = 0;
    private string hand = "";

    public GameObject PIDUI;
    public GameObject HandUI;
    public GameObject room;
    public GameObject avatar;
    public GameObject fittsController;

    public int[,] conditionSquare = {{0, 1, 3, 2 },
                                     {1, 2, 0, 3 },
                                     {2, 3, 1, 0 },
                                     {3, 0, 2, 1 }};

    private int participantID = 1;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentStatus)
        {
            case PROGRAM_STATUS.TEST:
                break;
            case PROGRAM_STATUS.START:
                break;
            case PROGRAM_STATUS.PRACTICE:
                CheckClick();
                if (FittsVRController.fittsVRinstance.GetPracticeComplete()) buttonController.instance.SetActive(true);
                break;
            case PROGRAM_STATUS.TRIAL:
                CheckClick();
                if (FittsVRController.fittsVRinstance.GetTrialComplete()) NewStatus(PROGRAM_STATUS.POST_TRIAL);
                break;
            case PROGRAM_STATUS.POST_TRIAL:
                break;
            case PROGRAM_STATUS.END:
                break;
        }
    }

    private void CheckClick()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) && !triggerDown)
        {
            FittsVRController.fittsVRinstance.TargetSelected(Vector3.one);
            triggerDown = true;
        }

        if ((Input.GetKeyUp(KeyCode.Space) || OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger)) && triggerDown)
        {
            triggerDown = false;
        }
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
            NewStatus(PROGRAM_STATUS.END);
        }
    }

    public void NextButton()
    {
        switch (currentStatus)
        {
            case PROGRAM_STATUS.PRACTICE:
                FittsVRController.fittsVRinstance.SetPractice(false);
                FittsVRController.fittsVRinstance.StartTrials();
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
                SetCondition();
                FittsVRController.fittsVRinstance.StartFitts();
                currentStatus = PROGRAM_STATUS.TRIAL;
                buttonController.instance.SetActive(false);
                break;
            case PROGRAM_STATUS.POST_TRIAL:
                buttonController.instance.SetActive(true);
                currentStatus = PROGRAM_STATUS.POST_TRIAL;
                break;
            case PROGRAM_STATUS.END:
                FittsVRController.fittsVRinstance.EndTrials();
                buttonController.instance.SetEnd();
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
