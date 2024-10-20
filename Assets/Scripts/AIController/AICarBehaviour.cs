using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
using System.Security.Cryptography;
//This script detects the AI car speed to see if the car it’s stuck so it will start going reverse for 1 second to get back on track
//Also, it uses a Box collider with IsTrigger option checked to see if an AI Car or the player car are in front of this car and it will brake
public class AICarBehaviour : MonoBehaviour
{
    AIController AIController;
    private Rigidbody AICarRigidbody;
    private float AICarSpeed;
    private int CheckReverse, StartReverse;
    private float NormalTorque, NormalSteering, NormalTopspeed;
    private WheelCollider[] AllWheelColliders;

    void Start()
    {
        StartCoroutine(CheckReverseCoroutine());
        AICarRigidbody = GetComponent<Rigidbody>();
        AIController = gameObject.GetComponentInParent(typeof(AIController)) as AIController;
        AllWheelColliders = GetComponentsInChildren<WheelCollider>();
        NormalTorque = AIController.m_FullTorqueOverAllWheels;
        NormalSteering = AIController.m_MaximumSteerAngle;
        NormalTopspeed = AIController.m_Topspeed;
    }

    IEnumerator CheckReverseCoroutine()
    {
        //the script will wait 3 seconds so the AI car can get enough speed after starting the race
        //otherwise if you don't wait, the car always start at 0 speed so it will trigger the reverse coroutine
        yield return new WaitForSeconds(3);
        CheckReverse = 1;//now that 3 seconds passed we can check if the AI car needs to use the 1 second reverse
        yield return null;
    }

    private void Update()
    {
        AICarSpeed = AICarRigidbody.velocity.magnitude;//speed of the AI car
        if (AICarSpeed < 0.25f && CheckReverse == 1)//if the AI car speed goes to a near zero speed (0.25f)
        {
            CheckReverse = 0;//stop checking if it needs a reverse
            StartReverse = 1;//and start one
        }

        if (StartReverse == 1)//starting the reverse:
        {
            //invert the torque of the car so it starts going in reverse
            AIController.m_FullTorqueOverAllWheels = (AIController.m_FullTorqueOverAllWheels * -1);
            AIController.m_MaximumSteerAngle = 0;//the car won't turn
            StartReverse = 0;//leave this deactivated so we can use it again in future reverses on the same race
            StartCoroutine(ReverseCoroutine());//start the reverse coroutine
        }
    }

    IEnumerator ReverseCoroutine()
    {      
        foreach (WheelCollider wc in AllWheelColliders)       
            wc.enabled = false;
        foreach (WheelCollider wc in AllWheelColliders)
            wc.enabled = true;
        //the reverse coroutine will wait 1 second with the car going in reverse and without turning
        yield return new WaitForSeconds(1);
        //after that, it will bring back the normal torque of the car so it goes forward again
        AIController.m_FullTorqueOverAllWheels = AIController.m_FullTorqueOverAllWheels + NormalTorque + NormalTorque;
        //after one second, the car will be able to turn again
        yield return new WaitForSeconds(1);
        AIController.m_MaximumSteerAngle = NormalSteering;
        //and after one last second, we can now check again if the AI car needs another reverse
        yield return new WaitForSeconds(1);
        CheckReverse = 1;
        yield return null;
    }

    //check if there's a player or AI car inside the box collider and brake
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "AICarCollider")
        {
            if(col.gameObject.GetComponentInParent<AIController>().isSlow == true)
            {
                Vector3 newPosition = col.transform.position + col.transform.forward * 5f;
                transform.position = newPosition;
            }
            else
            {
                AIController.m_Topspeed = 15;//brakes are actually reducing the topspeed of the car
            }
        }
        if(col.tag == "Slow")
        {
            AIController.isSlow = true;
            AIController.m_Topspeed = 15;
            Invoke("ResetSpeed", 5f);
        }
    }
    //if the AI/player car exits the box collider:
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "AICarCollider")
        {
            AIController.m_Topspeed = NormalTopspeed;//bring back the normal topspeed
        }
    }
    void ResetSpeed()
    {
        AIController.isSlow = false;
        AIController.m_Topspeed = NormalTopspeed;
    }
}

