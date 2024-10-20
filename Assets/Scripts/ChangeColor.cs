using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class ChangeColor : MonoBehaviour
{
    private MeshRenderer mRenderer;
    private AIController ac;
    private bool toggle = false;
    private bool isInvoke = false;

    void Start()
    {
        mRenderer = GetComponentInChildren<MeshRenderer>();
        ac = GetComponent<AIController>();
        toggle = false;
    }

    void Update()
    {
        if (ac.isSlow && !isInvoke)  
        {
            InvokeRepeating("ToggleColor", 0f, 0.1f);
            isInvoke = true;
        }
        
        if(!ac.isSlow)
        {
            CancelInvoke("ToggleColor");
            mRenderer.material.color = new Color(1, 1, 1, 1);
        }
        
    }
    void ToggleColor()
    {
        if (toggle)
        {
            mRenderer.material.color = new Color(1, 1, 1, 1);
        }
        else
        {
            mRenderer.material.color = new Color(0, 0, 0, 1);
        }
        toggle = !toggle;
    }
}
