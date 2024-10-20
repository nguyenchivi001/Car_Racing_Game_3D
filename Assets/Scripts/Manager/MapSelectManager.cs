using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityStandardAssets.Vehicles.Car;

public class MapSelectManager : MonoBehaviour
{

    [Header("Car Select Canvas")]
    public ListCar[] listCarSets;
    public GameObject selectMapCanvas;
    public GameObject betMenu;
    public GameObject errorCanvas;
    public TextMeshProUGUI errorText;
    public GameObject rightButton;
    public GameObject leftButton;
    public ListMap listOfMap;
    public TextMeshProUGUI nameOfMap;
    public GameObject rotateTurnTable;
    public TextMeshProUGUI currency;
    public TextMeshProUGUI[] carText;
    public TextMeshProUGUI[] butText;
    public TMP_InputField inputField1;
    public TMP_InputField inputField2;
    public TMP_InputField inputField3;


    [Header("SFX")]
    [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
    public AudioSource hoverSound;

    [HideInInspector] public float rotateSpeed = 10f;
    [HideInInspector] public int mapPointer = 0;
    [HideInInspector] public int round = 0;

    private int curCarSet = 0;
    private int index = 0;

    void Awake()
    {
        betMenu.SetActive(false);
        errorCanvas.SetActive(false);
        selectMapCanvas.SetActive(true);
        index = 0;
        currency.text = PlayerPrefs.GetInt("currency").ToString();
        mapPointer = PlayerPrefs.GetInt("mp",0);
        GameObject childObject = Instantiate(listOfMap.Maps[mapPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
        childObject.transform.parent = rotateTurnTable.transform;
        childObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        if(mapPointer == 0)
        {
            leftButton.SetActive(false);
        }
        if(mapPointer == listOfMap.Maps.Length - 1)
        {
            rightButton.SetActive(false);
        }
        GetMapInfo();
    }

    private void Update() 
    {
        rotateTurnTable.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        if(index == 3)
        {
            GameObject[] chooseObjects = GameObject.FindGameObjectsWithTag("choose");
            foreach (GameObject obj in chooseObjects)
            {
                obj.SetActive(false);
            }
        }
    }

    public void RightButtonClicked()
    {
        betMenu.SetActive(false);
        if(mapPointer < listOfMap.Maps.Length - 1)
        {
            leftButton.SetActive(true);
            if(mapPointer < listOfMap.Maps.Length - 2)
            {
                rightButton.SetActive(true);
            }
            else
            {
                rightButton.SetActive(false);
            }
            Destroy(GameObject.FindGameObjectWithTag("Map"));
            mapPointer++;
            PlayerPrefs.SetInt("mp",mapPointer);
            GameObject childObject = Instantiate(listOfMap.Maps[mapPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
            childObject.transform.parent = rotateTurnTable.transform;
            childObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            GetMapInfo();
        }
    }

    public void LeftButtonClicked()
    {
        betMenu.SetActive(false);
        if(mapPointer > 0)
        {
            rightButton.SetActive(true);
            if(mapPointer > 1)
            {
                leftButton.SetActive(true);
            }
            else 
            {
                leftButton.SetActive(false);
            }
            Destroy(GameObject.FindGameObjectWithTag("Map"));
            mapPointer--;
            PlayerPrefs.SetInt("mp",mapPointer);
            GameObject childObject = Instantiate(listOfMap.Maps[mapPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
            childObject.transform.parent = rotateTurnTable.transform;
            childObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            GetMapInfo();
        }
    }
    public void GetMapInfo()
    {
        int mapIndex = PlayerPrefs.GetInt("mp");
        MapModifier MM = listOfMap.Maps[mapIndex].GetComponent<MapModifier>();

        nameOfMap.text = MM.mapName;
    }

    public void Lv1ButtonClicked()
    {
        betMenu.SetActive(true);
        round = 1;
        PlayerPrefs.SetInt("Round", round);
        curCarSet = PlayerPrefs.GetInt("chPointer");
        for(int i = 0; i < listCarSets[curCarSet].Cars.Length; i++)
        {
            GameObject car = listCarSets[curCarSet].Cars[i];
            AIController ac = car.GetComponent<AIController>();
            butText[i].text = ac.carName;
        }

    }
    public void Lv2ButtonClicked()
    {
        betMenu.SetActive(true);
        round = 2;
        PlayerPrefs.SetInt("Round", round);
        curCarSet = PlayerPrefs.GetInt("chPointer");
        for(int i = 0; i < listCarSets[curCarSet].Cars.Length; i++)
        {
            GameObject car = listCarSets[curCarSet].Cars[i];
            AIController ac = car.GetComponent<AIController>();
            butText[i].text = ac.carName;
        }
    }
    public void Lv3ButtonClicked()
    {
        betMenu.SetActive(true);
        round = 3;
        PlayerPrefs.SetInt("Round", round);
        curCarSet = PlayerPrefs.GetInt("chPointer");
        for(int i = 0; i < listCarSets[curCarSet].Cars.Length; i++)
        {
            GameObject car = listCarSets[curCarSet].Cars[i];
            AIController ac = car.GetComponent<AIController>();
            butText[i].text = ac.carName;
        }
    }
    public void BackButtonClicked()
    {
        SceneManager.LoadScene("MainScene");
    }
    
    public void ChooseButtonClicked(Button button)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            carText[index].text = buttonText.text;
        }
        index++;
        button.gameObject.SetActive(false);
    }
    public void BetPlayButtonClicked()
    {
        int coin1,coin2,coin3;
        try
        {
            coin1 = int.Parse(inputField1.text);
            coin2 = int.Parse(inputField2.text);
            coin3 = int.Parse(inputField3.text);
        }
        catch (FormatException)
        {
            ShowErrorMessage("Invalid input value");
            Invoke("CloseErrorMessage",1.5f);
            return;
        }
        if(coin1 % 100 != 0 || coin1 == 0 || coin2 % 100 != 0 || coin2 == 0 || coin3 % 100 != 0 || coin3 == 0)
        {
            ShowErrorMessage("Entered must be a multiple of 100.");
            Invoke("CloseErrorMessage",1.5f);
            return;
        }
        int currentCoin = PlayerPrefs.GetInt("currency");
        int coin = coin1 + coin2 + coin3;
        if(coin1+coin2+coin3 > currentCoin)
        {
            ShowErrorMessage("Not enough currency");
            Invoke("CloseErrorMessage",1.5f);
        }
        else
        {
            PlayerPrefs.SetInt("Betcoin1", coin1);
            PlayerPrefs.SetInt("Betcoin2", coin2);
            PlayerPrefs.SetInt("Betcoin3", coin3);
            PlayerPrefs.SetString("CarTop1Bet", carText[0].text);
            PlayerPrefs.SetString("CarTop2Bet", carText[1].text);
            PlayerPrefs.SetString("CarTop3Bet", carText[2].text);
            Debug.Log(PlayerPrefs.GetInt("Betcoin1") + " " + PlayerPrefs.GetInt("Betcoin2") + " " + PlayerPrefs.GetInt("Betcoin3"));
            int mapIndex = PlayerPrefs.GetInt("mp");
            MapModifier MM = listOfMap.Maps[mapIndex].GetComponent<MapModifier>();
            SceneManager.LoadScene(MM.mapName);
            PlayerPrefs.SetInt("StartGame",1);
        }
    }

    public void BetBackButtonClicked()
    {
        betMenu.SetActive(false);
        index = 0;
    }

    public void PlayHover()
    {
		hoverSound.Play();
	}

    private void ShowErrorMessage(string message)
    {
        errorText.text = "" + message;
        errorCanvas.SetActive(true);
    }
    
    public void CloseErrorMessage()
    {
        errorText.text = "";
        errorCanvas.SetActive(false);
    }
}
