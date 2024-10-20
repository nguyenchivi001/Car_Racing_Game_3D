using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int coin = 0;

    public int lastChoose = 0;
    public int[] OwnedCar = new int[9];

    public PlayerData()
    {
        coin = 5000;
        lastChoose = 0;
        for (int i = 0; i < 5; i++)
        {
            OwnedCar[i] = 1;
        }
        for (int i = 5; i < 9; i++)
        {
            OwnedCar[i] = 0;
        }
    }
}
