using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FittsVRController : MonoBehaviour
{
    public static FittsVRController fittsVRinstance;

    public int inputTotalTargets = 0;
    public float inputAmplitude = 0.0f;
    public float inputTargetWidth = 1.0f;

    public GameObject targetPrefab;
    public Material targetBasicMaterial;
    public Material targetActiveMaterial;
    public Material targetInactiveMaterial;

    public GameObject targetContainer;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(inputTotalTargets != currentTotalTargets || inputAmplitude != currentAmplitude || inputTargetWidth != currentTargetWidth)
        {
            currentTotalTargets = inputTotalTargets;
            currentAmplitude = inputAmplitude;
            currentTargetWidth = inputTargetWidth;
            ResetTargets();
        }

        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Vector3 controllerPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            Vector2 controllerPos2d = new Vector2(controllerPos.x, controllerPos.y);
            Vector2 targetPos = new Vector2(targets[0].transform.position.x, targets[0].transform.position.y);

            float selectDistance = Vector2.Distance(controllerPos2d, targetPos);

            Debug.Log(selectDistance + ", " + currentTargetWidth);
        }
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
        if (targetCount > 0)
        {
            targets[currentTargetIndex].gameObject.GetComponent<Renderer>().material = targetInactiveMaterial;
            int halfWay = (currentTotalTargets + 1) / 2;
            currentTargetIndex = (currentTargetIndex + halfWay) % currentTotalTargets;
        }
        targets[currentTargetIndex].gameObject.GetComponent<Renderer>().material = targetActiveMaterial;
    }

    public void TargetSelected(Vector3 selectionVector)
    {
        targetCount++;

        // Output:
        //  ID
        //  A
        //  W
        //  Time
        //  Selection V3
        //  Target V3

        if(targetCount >= currentTotalTargets)
        {
            // Go to next trail
        } else
        {
            SetNextActiveTarget();
        }

    }
}
