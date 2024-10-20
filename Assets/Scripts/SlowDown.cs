using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class SlowDown : MonoBehaviour
{
    private float normalTopspeed;
    private MeshRenderer carRenderer;
    AIController ac;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "AICarCollider")
        {
            Destroy(gameObject);
        }
    }
}
