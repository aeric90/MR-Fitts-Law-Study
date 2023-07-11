using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandUIController : MonoBehaviour
{
    public static HandUIController instance;
    public GameObject leftButton;
    public GameObject rightButton;
    public string hand = "";

    private void Start()
    {
        instance = this;
    }

    public void clickLeft()
    {
        leftButton.GetComponent<Image>().color = Color.green;
        rightButton.GetComponent<Image>().color = Color.white;
        hand = "left";
    }

    public void clickRight()
    {
        leftButton.GetComponent<Image>().color = Color.white;
        rightButton.GetComponent<Image>().color = Color.green;
        hand = "right";
    }
}
