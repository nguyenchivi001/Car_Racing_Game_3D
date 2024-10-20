using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car 
{
    public int node;
    public string name;
    public bool hasFinished;

    public Car(int node,string name,bool hasFinished)
    {
        this.node = node;
        this.name = name;
        this.hasFinished = hasFinished;
    }
}