using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;
using Cinemachine;

public class GameManager : MonoBehaviour
{

    public ListCar listCar1;
    public ListCar listCar2;
    public ListCar listCar3;
    public GameObject[] aiCars;
    private AICarControl[] aiCarControlScripts;
    private AICarBehaviour[] aiCarBehaviourScripts;
    private AIController[] aiControllersScripts;
    private int currentIndex = 0;
    public GameObject startPosition1;
    public GameObject startPosition2;
    public GameObject startPosition3;
    public GameObject startPosition4;
    public GameObject startPosition5;
    public GameObject cDPanel;
    public TextMeshProUGUI cooldownText;
    public GameObject notifCanvas;
    public GameObject pauseMenuUI;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI carNameText;
    public GameObject timeLapCanvas;
    public GameObject finishCanvas;
    public TextMeshProUGUI top1;
    public TextMeshProUGUI top2;
    public TextMeshProUGUI top3;
    public bool isFinish = false;
    private bool isPaused = false;
    private bool isStart = false;
    public float timeLap = 0f;
    private int round = 0;
    private int listCar = 0;
    private RaceFinish raceFinish;
    private float cooldownTime = 3f;
    private CinemachineVirtualCamera CVC; 
    private GameObject CameraLookAt, CameraFollow;
    private bool finishCheck = false;

    [Header("SFX")]
    [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
    public AudioSource hoverSound;

    void Awake()
    {   
        isFinish = false;
        cDPanel.SetActive(true);
        timeLapCanvas.SetActive(false);
        finishCanvas.SetActive(false);
        notifCanvas.SetActive(false);
        pauseMenuUI.SetActive(false);
        listCar = 1;
        if(listCar == 1)
        {
            Instantiate(listCar1.Cars[0], startPosition1.transform.position, startPosition1.transform.rotation);
            Instantiate(listCar1.Cars[1], startPosition2.transform.position, startPosition2.transform.rotation);
            Instantiate(listCar1.Cars[2], startPosition3.transform.position, startPosition3.transform.rotation);
            Instantiate(listCar1.Cars[3], startPosition4.transform.position, startPosition4.transform.rotation);
            Instantiate(listCar1.Cars[4], startPosition5.transform.position, startPosition5.transform.rotation);   
        }
        else if(listCar == 2)
        {
            Instantiate(listCar2.Cars[0], startPosition1.transform.position, startPosition1.transform.rotation);
            Instantiate(listCar2.Cars[1], startPosition2.transform.position, startPosition2.transform.rotation);
            Instantiate(listCar2.Cars[2], startPosition3.transform.position, startPosition3.transform.rotation);
            Instantiate(listCar2.Cars[3], startPosition4.transform.position, startPosition4.transform.rotation);
            Instantiate(listCar2.Cars[4], startPosition5.transform.position, startPosition5.transform.rotation);
        }
        else if(listCar == 3)
        {
            Instantiate(listCar3.Cars[0], startPosition1.transform.position, startPosition1.transform.rotation);
            Instantiate(listCar3.Cars[1], startPosition2.transform.position, startPosition2.transform.rotation);
            Instantiate(listCar3.Cars[2], startPosition3.transform.position, startPosition3.transform.rotation);
            Instantiate(listCar3.Cars[3], startPosition4.transform.position, startPosition4.transform.rotation);
            Instantiate(listCar3.Cars[4], startPosition5.transform.position, startPosition5.transform.rotation);
        }

        aiCars = GameObject.FindGameObjectsWithTag("AI");
        aiCarControlScripts = FindObjectsOfType(typeof(AICarControl)) as AICarControl[];
        aiCarBehaviourScripts = FindObjectsOfType(typeof(AICarBehaviour)) as AICarBehaviour[];
        aiControllersScripts = FindObjectsOfType(typeof(AIController)) as AIController[];
        System.Random random = new System.Random();
        foreach (AIController ac in aiControllersScripts)
        {
            ac.hasFinished = false;
            int randomFullTorque = random.Next(400, 2001);
            int randomMaxSpeed = random.Next(50,201);
            ac.m_FullTorqueOverAllWheels = randomFullTorque;
            ac.m_Topspeed = randomMaxSpeed;
        }

        currentIndex = 0;
        CVC = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        CameraLookAt = aiCars[currentIndex].transform.Find ("Camera lookat").gameObject;
        CameraFollow = aiCars[currentIndex].transform.Find ("Camera constraint").gameObject;
        CVC.LookAt = CameraLookAt.transform;
        CVC.Follow = CameraFollow.transform;
        carNameText.text = aiCars[currentIndex].GetComponent<AIController>().carName;

        raceFinish = GameObject.FindGameObjectWithTag ("Finish").GetComponent<RaceFinish>();
        round = PlayerPrefs.GetInt("Round", 0);
    }
    private void Update () 
    {
        //Start Game
        if(cooldownTime > 0)
        {
            Cooldown();
            AICarControlOff();
        }
        else
        {
            if(!isStart)
            {
                cooldownText.text = "Start";
                Invoke("StartGame",0.5f);
            }
        }
        //Pause Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
        //Change Camera 
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentIndex--;
            if(currentIndex < 0) currentIndex = aiCars.Length - 1;
            CameraLookAt = aiCars[currentIndex].transform.Find ("Camera lookat").gameObject;
            CameraFollow = aiCars[currentIndex].transform.Find ("Camera constraint").gameObject;
            CVC.LookAt = CameraLookAt.transform;
            CVC.Follow = CameraFollow.transform;
            carNameText.text = aiCars[currentIndex].GetComponent<AIController>().carName;
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentIndex++;
            if(currentIndex >= aiCars.Length) currentIndex = 0;
            CameraLookAt = aiCars[currentIndex].transform.Find ("Camera lookat").gameObject;
            CameraFollow = aiCars[currentIndex].transform.Find ("Camera constraint").gameObject;
            CVC.LookAt = CameraLookAt.transform;
            CVC.Follow = CameraFollow.transform;
            carNameText.text = aiCars[currentIndex].GetComponent<AIController>().carName;
        }

        if(isFinish == true && finishCheck == false)
        {
            finishCheck = true;
            timeLapCanvas.SetActive(false);
            finishCanvas.SetActive(true);
            GameObject carTop1 = raceFinish.resultsCar[0].transform.parent.gameObject;
            GameObject carTop2 = raceFinish.resultsCar[1].transform.parent.gameObject;
            GameObject carTop3 = raceFinish.resultsCar[2].transform.parent.gameObject;
            top1.text = "Top 1: " + carTop1.GetComponent<AIController>().carName;
            top2.text = "Top 2: " + carTop2.GetComponent<AIController>().carName;
            top3.text = "Top 3: " + carTop3.GetComponent<AIController>().carName;
            PlayerPrefs.SetString("CarTop1Res", top1.text);
            PlayerPrefs.SetString("CarTop2Res", top2.text);
            PlayerPrefs.SetString("CarTop3Res", top3.text);
            Invoke("ReturnMainScene",5f);
        }
        UpdateTimeLap();
    }
    public void AICarControlOn()
    {
        foreach (AICarControl acc in aiCarControlScripts)
        {
            acc.enabled = true;
        }
        foreach (AICarBehaviour acb in aiCarBehaviourScripts)
        {
            acb.enabled = true;
        }
    }
    public void AICarControlOff()
    {
        foreach (AICarControl acc in aiCarControlScripts)
        {
            acc.enabled = false;
        }
        foreach (AICarBehaviour acb in aiCarBehaviourScripts)
        {
            acb.enabled = false;
        }
    }
    
    private void UpdateTimeLap()
    {
        if(isStart)
        {
            if(!isFinish)
            {
                timeLap += Time.deltaTime;
                TimeSpan timeSpan = TimeSpan.FromSeconds(timeLap);
                string formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}.{3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                timeText.text = formattedTime;
            }
        }
        
    }

    public void StartGame () 
    {
        cDPanel.SetActive(false);
        timeLapCanvas.SetActive(true);
        isStart = true;
        AICarControlOn();
    }

    public void Cooldown()
    {
        int cd = (int)cooldownTime;
        cooldownText.text = cd.ToString();
        cooldownTime -= Time.deltaTime;
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        isPaused = true;
        isStart = false;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        isPaused = false;
        isStart = true;
    }
    public void ReturnBut()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }
    public void ReturnMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void PlayHover()
    {
		hoverSound.Play();
	}
}
