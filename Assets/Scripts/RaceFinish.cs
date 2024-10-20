using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityStandardAssets.Vehicles.Car;
using Unity.Mathematics;

public class RaceFinish : MonoBehaviour
{
    public int round = 0;
    public GameManager gameManager;
    public GameObject notifCanvas;
    public TextMeshProUGUI notifText;
    public TextMeshProUGUI[] resultsText;
    public GameObject[] resultsCar;
    public int curIndex = 0;

    void Awake()
    {
        round = PlayerPrefs.GetInt("Round");
        curIndex = 0;
        resultsCar = new GameObject[5];
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "AICarCollider")
        {
            GameObject child = other.gameObject;
            AIController ac = child.transform.parent.gameObject.GetComponent<AIController>();
            if(ac.hasFinished == true)
            {
                resultsCar[curIndex] = other.gameObject;
                TimeSpan timeSpan = TimeSpan.FromSeconds(gameManager.timeLap);
                string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}.{3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                resultsText[curIndex].text = (curIndex + 1).ToString() + "   " + ac.carName + "   " + formattedTime;
                curIndex++;
                if(curIndex >= 5)
                {
                    gameManager.isFinish = true;
                }
            }
        }
    }
    private void ShowNotifMessage(string message)
    {
        notifText.text = "" + message;
        notifCanvas.SetActive(true);
    }
    
    public void CloseNotifMessage()
    {
        notifText.text = "";
        notifCanvas.SetActive(false);
    }
}
