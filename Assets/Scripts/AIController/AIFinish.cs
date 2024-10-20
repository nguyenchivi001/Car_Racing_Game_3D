using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class AIFinish : MonoBehaviour
{
    public int round = 0;
    private AICarBehaviour acb;
    private AICarControl acc;
    private AIController ac;


    void Awake()
    {
        //round = PlayerPrefs.GetInt("Round");
        round = 1;
        acb = GetComponent<AICarBehaviour>();
        acc = GetComponent<AICarControl>();
        ac = GetComponent<AIController>();
        ac.currentRound = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish")
        {
            ac.currentRound++;
        }
        if(ac.currentRound == round)
        {
            ac.hasFinished = true;
            ac.m_FullTorqueOverAllWheels = 100;
            ac.m_Topspeed = 15;
            Invoke("Exit", 2f);
        }
    }
    void Exit()
    {
        acb.enabled = false;
        acc.enabled = false;
    }
}
