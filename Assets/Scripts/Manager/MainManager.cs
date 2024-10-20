using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityStandardAssets.Vehicles.Car;
using Unity.VisualScripting;

public class UIMenuManager : MonoBehaviour
{
    [Header("Camera")]
    public GameObject cameraObject;
    public float lerpTime;
    public GameObject finalCameraPosition , startCameraPosition;

    [Header("MENUS")]
    [Tooltip("The Menu for when the MAIN menu buttons")]
    public GameObject mainCanvas;

    [Tooltip("The Canvas for when the Shop button is clicked")]
    public GameObject shopCanvas;

    [Tooltip("The Canvas for when the Inventory button is clicked")]
    public GameObject invCanvas;

    [Tooltip("Setting window")]
    public GameObject settingsCanvas;

    [Tooltip("The list of buttons")]
    public GameObject mainMenu;

    [Tooltip("The Menu for when the PLAY button is clicked")]
    public GameObject playMenu;

    [Tooltip("The Menu for when the EXIT button is clicked")]
    public GameObject exitMenu;

    [Header("PANELS")]
    [Tooltip("The UI Panel that holds the CONTROLS window tab")]
    public GameObject PanelControls;

    [Tooltip("The UI Panel that holds the GAME window tab")]
    public GameObject PanelGame;
    
    [Header("SETTINGS SCREEN")]
    [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
    public GameObject lineGame;

    [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
    public GameObject lineControls;

    public GameObject lineNormal;
    public GameObject lineHardcore;

	[Header("SFX")]

    [Tooltip("The GameObject holding the Audio Source component for the HOVER SOUND")]
    public AudioSource hoverSound;
    [Tooltip("The GameObject holding the Audio Source component for the AUDIO SLIDER")]
    public AudioSource sliderSound;
    [Tooltip("The GameObject holding the Audio Source component for the SWOOSH SOUND when switching to the Settings Screen")]
    public AudioSource swooshSound;

    [Header("OTHERS")]
    public GameObject notifCanvas;
    public TextMeshProUGUI notifText;
    public GameObject buyButton;
    public GameObject shopLeftButton;
    public GameObject shopRightButton;
    public GameObject invLeftButton;
    public GameObject invRightButton;
    public GameObject lockButton;
    public Image lockImage , unlockImage;
    public TextMeshProUGUI mainCurrency;
    public TextMeshProUGUI shopCurrency;
    public TextMeshProUGUI invCurrency;
    public TextMeshProUGUI carShopInfo;
    public TextMeshProUGUI carInvInfo;
    public ListCar listOfCars;
    public ListCar[] listCarSets;
    public GameObject rotateTurnTable;
    private bool finalToStart,startToFinal;

    [HideInInspector] public float rotateSpeed = 10f;
    [HideInInspector] public SaveLoadManager saveLoadManager;

    //PlayerPref
    [HideInInspector] public int carPointer;
    [HideInInspector] public int carSetPointer;
    [HideInInspector] public int coin;
    private int isLoad = 0;
    private int isHard = 0;
    private int chCarSet = 0;

    void Awake()
    {
        //Set active 

        mainCanvas.SetActive(true);
        shopCanvas.SetActive(false);
        invCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        notifCanvas.SetActive(false);
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
		exitMenu.SetActive(false);
        startToFinal = true;
        finalToStart = false;
        
        isLoad = PlayerPrefs.GetInt("IsLoad",0);
        if(isLoad == 1)
        {
            carSetPointer = PlayerPrefs.GetInt("csPointer",0);
            coin = PlayerPrefs.GetInt("currency",0);
            carPointer = Random.Range(0, 9);
            isHard = PlayerPrefs.GetInt("IsHard",0);
            chCarSet = PlayerPrefs.GetInt("chPointer",0);

            int isStartGame = PlayerPrefs.GetInt("StartGame",0);
            if(isStartGame == 1)
            {
                //cal bet
                int betCoin1 = PlayerPrefs.GetInt("Betcoin1",0);
                int betCoin2 = PlayerPrefs.GetInt("Betcoin2",0);
                int betCoin3 = PlayerPrefs.GetInt("Betcoin3",0);
                string car1Bet =  PlayerPrefs.GetString("CarTop1Bet");
                string car2Bet =  PlayerPrefs.GetString("CarTop2Bet");
                string car3Bet =  PlayerPrefs.GetString("CarTop3Bet");
                string car1Res = PlayerPrefs.GetString("CarTop1Res");
                string car2Res = PlayerPrefs.GetString("CarTop2Res");
                string car3Res = PlayerPrefs.GetString("CarTop3Res");
                int result = 0;
                if(car1Bet == car1Res)
                {
                    result += betCoin1;
                }
                else 
                {
                    result -= betCoin1;
                }
                if(car2Bet == car2Res)
                {
                    result += betCoin2;
                }
                else 
                {
                    result -= betCoin2;
                }
                if(car3Bet == car3Res)
                {
                    result += betCoin3;
                }
                else 
                {
                    result -= betCoin3;
                }
                if(result >= 0)
                {
                    string text = "You gain: " + result.ToString();
                    ShowNotifMessage(text);
                    Invoke("CloseNotifMessage",2f);
                }
                else
                {
                    string text = "You lose: " + (-result).ToString();
                    ShowNotifMessage(text);
                    Invoke("CloseNotifMessage",2f);
                }
                coin += result;
                PlayerPrefs.SetInt("StartGame",0);
            }
            

        }
        //load/save data`
        //PlayerPrefs.DeleteAll();
        saveLoadManager = FindObjectOfType<SaveLoadManager>();
        if (saveLoadManager == null)
        {
            Debug.LogError("Không tìm thấy đối tượng SaveLoadManager trong Scene!");
        }
        isLoad = PlayerPrefs.GetInt("IsLoad",0);
        Debug.Log("IsLoad = " + isLoad);
        if(isLoad == 0)
        {
            coin = saveLoadManager.playerData.coin;
            carSetPointer = saveLoadManager.playerData.lastChoose;
            chCarSet = carSetPointer;
            carPointer = Random.Range(1, 10);
            InitOwnedCarData();

            isLoad = 1;
            PlayerPrefs.SetInt("IsLoad",isLoad);
        }

        //Save Data vào PlayerPref
        PlayerPrefs.SetInt("currency", coin);
        PlayerPrefs.SetInt("carPointer", carPointer);
        PlayerPrefs.SetInt("csPointer", carSetPointer);
        PlayerPrefs.SetInt("chPointer", chCarSet);

        //hiển thị giao diện
        mainCurrency.text = PlayerPrefs.GetInt("currency").ToString();
        GameObject childObject = Instantiate(listOfCars.Cars[carPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
        childObject.transform.parent = rotateTurnTable.transform;

        //childObject.GetComponent<AIController>().enabled = false;
        childObject.GetComponent<AICarControl>().enabled = false;
        childObject.GetComponent<AICarBehaviour>().enabled = false;

        saveLoadManager.playerData.coin = PlayerPrefs.GetInt("currency");
        saveLoadManager.playerData.lastChoose = PlayerPrefs.GetInt("csPointer");
        SaveOwnedCarData();
        saveLoadManager.SaveData();
    }

    public void InitOwnedCarData()
    {
        for (int i = 0; i < saveLoadManager.playerData.OwnedCar.Length; i++)
        {
            PlayerPrefs.SetInt("Car_" + i, saveLoadManager.playerData.OwnedCar[i]);
        }
    }

    public void SaveOwnedCarData()
    {
        for (int i = 0; i < saveLoadManager.playerData.OwnedCar.Length; i++)
        {
            saveLoadManager.playerData.OwnedCar[i] = PlayerPrefs.GetInt("Car_" + i);
        }
    }

    private void FixedUpdate() 
    {
        rotateTurnTable.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        CameraTranzition();
    }

    public void CameraTranzition()
    {
        if(startToFinal)
        {
            cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position,finalCameraPosition.transform.position,lerpTime * Time.deltaTime); 
        }
        if(finalToStart)
        {
            cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position,startCameraPosition.transform.position,lerpTime * Time.deltaTime); 
        }
    }

    //Setting button

    //main canvas

    public void  DisableMain()
    {
        mainMenu.SetActive(false);
		playMenu.SetActive(false);
        exitMenu.SetActive(false);
	}
    public void PlayButtonClicked()
    {
        exitMenu.SetActive(false);
        if(!playMenu.activeSelf)
		{
            playMenu.SetActive(true);
        }
        else 
        {
            playMenu.SetActive(false);
        }
	}

    public void ShopButtonClicked()
    {
        mainCanvas.SetActive(false);
        playMenu.SetActive(false);
        exitMenu.SetActive(false);
        shopCanvas.SetActive(true);
        finalToStart = true;
        startToFinal = false;
        
        shopCurrency.text = saveLoadManager.playerData.coin.ToString();

        InitOwnedCarData();

        if(carPointer == 0)
        {
            shopLeftButton.SetActive(false);
        }
        if(carPointer == listOfCars.Cars.Length - 1)
        {
            shopRightButton.SetActive(false);
        }

        GetCarInfo();
    }
    public void InventoryButtonClicked()
    {
        mainCanvas.SetActive(false);
        playMenu.SetActive(false);
        shopCanvas.SetActive(false);
        exitMenu.SetActive(false);
        invCanvas.SetActive(true);
        finalToStart = true;
        startToFinal = false;

        invCurrency.text = saveLoadManager.playerData.coin.ToString();

        carSetPointer = PlayerPrefs.GetInt("csPointer",0);

        carPointer = 0;
        PlayerPrefs.SetInt("carPointer",carPointer);

        invLeftButton.SetActive(false);

        lockImage.gameObject.SetActive(true);
        unlockImage.gameObject.SetActive(false);

        Destroy(GameObject.FindGameObjectWithTag("AI"));
        GameObject childObject = Instantiate(listCarSets[carSetPointer].Cars[carPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
        childObject.transform.parent = rotateTurnTable.transform;

        childObject.GetComponent<AICarControl>().enabled = false;
        childObject.GetComponent<AICarBehaviour>().enabled = false;
        InvCarInfo();
    }
    public void SettingButtonClicked()
    {
        mainCanvas.SetActive(false);
        playMenu.SetActive(false);
        exitMenu.SetActive(false);
        settingsCanvas.SetActive(true);
        PanelGame.SetActive(true);
        lineGame.SetActive(true);
        PanelControls.SetActive(false);
        lineControls.SetActive(false);
        if(isHard == 0)
        {
            lineNormal.SetActive(true);
            lineHardcore.SetActive(false);
        }
        else if(isHard == 1)
        {
            lineNormal.SetActive(false);
            lineHardcore.SetActive(true);
        }
    }

    public void ExitButtonClicked()
    {
        playMenu.SetActive(false);
        exitMenu.SetActive(true);
    }   

    public void ReturnMenu()
    {
        mainCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
        PanelGame.SetActive(false);
        lineGame.SetActive(false);
        PanelControls.SetActive(false);
        lineControls.SetActive(false);
		playMenu.SetActive(false);
		exitMenu.SetActive(false);
		mainMenu.SetActive(true);
	}
    
    //Play canvas
    public void NewGameButtonClicked()
    {
        saveLoadManager.playerData.coin = PlayerPrefs.GetInt("currency");
        SaveOwnedCarData();
        SceneManager.LoadScene("MapSelectScene");
    }
    public void MiniGameButtonClicked()
    {
        saveLoadManager.playerData.coin = PlayerPrefs.GetInt("currency");
        SaveOwnedCarData();
        SceneManager.LoadScene("FortuneWheel");
    }
    //Shop canvas
    public void ShopRightButtonClicked()
    {
        if(carPointer < listOfCars.Cars.Length - 1)
        {
            shopLeftButton.SetActive(true);
            if(carPointer < listOfCars.Cars.Length - 2)
            {
                shopRightButton.SetActive(true);
            }
            else
            {
                shopRightButton.SetActive(false);
            }
            Destroy(GameObject.FindGameObjectWithTag("AI"));
            carPointer++;
            PlayerPrefs.SetInt("carPointer",carPointer);
            GameObject childObject = Instantiate(listOfCars.Cars[carPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
            childObject.transform.parent = rotateTurnTable.transform;

            childObject.GetComponent<AICarControl>().enabled = false;
            childObject.GetComponent<AICarBehaviour>().enabled = false;
            GetCarInfo();
        }
    }
    public void ShopLeftButtonClicked()
    {
        if(carPointer > 0)
        {
            shopRightButton.SetActive(true);
            if(carPointer > 1)
            {
                shopLeftButton.SetActive(true);
            }
            else 
            {
                shopLeftButton.SetActive(false);
            }
            Destroy(GameObject.FindGameObjectWithTag("AI"));
            carPointer--;
            PlayerPrefs.SetInt("carPointer",carPointer);

            GameObject childObject = Instantiate(listOfCars.Cars[carPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
            childObject.transform.parent = rotateTurnTable.transform;

            childObject.GetComponent<AICarControl>().enabled = false;
            childObject.GetComponent<AICarBehaviour>().enabled = false;
            GetCarInfo();
        }
    }
    public void BuyButtonClicked()
    {
        int carIndex = PlayerPrefs.GetInt("carPointer");
        AIController ac = listOfCars.Cars[carIndex].GetComponent<AIController>();
        int currentCoin = PlayerPrefs.GetInt("currency");

        if(currentCoin >= ac.carPrice)
        {
            currentCoin -= ac.carPrice;

            PlayerPrefs.SetInt("currency", currentCoin);

            PlayerPrefs.SetInt("Car_" + carIndex, 1);

            GetCarInfo();
            ShowNotifMessage("Buy success!");
            Invoke("CloseNotifMessage",1.5f);
        }
        else
        {
            ShowNotifMessage("Not enough currency");
            Invoke("CloseNotifMessage",1.5f);
        }
    }
    public void GetCarInfo()
    {
        int carIndex = PlayerPrefs.GetInt("carPointer");
        AIController ac = listOfCars.Cars[carIndex].GetComponent<AIController>();

        shopCurrency.text = PlayerPrefs.GetInt("currency").ToString();

        if (PlayerPrefs.GetInt("Car_" + carIndex) == 1)
        {
            carShopInfo.text = ac.carName;
            buyButton.SetActive(false);
        }
        else
        {
            carShopInfo.text = ac.carName + " " + ac.carPrice;
            buyButton.SetActive(true);
        }
    }

    public void ShopReturnButtonClicked()
    {
        shopCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        mainMenu.SetActive(true);

        startToFinal = true;
        finalToStart = false;

        saveLoadManager.playerData.coin = PlayerPrefs.GetInt("currency");
        saveLoadManager.playerData.lastChoose = PlayerPrefs.GetInt("csPointer");
        SaveOwnedCarData();

        mainCurrency.text = saveLoadManager.playerData.coin.ToString();
        saveLoadManager.SaveData();
    }

    //Inventory canvas
    public void InvRightButtonClicked()
    {
        carSetPointer = PlayerPrefs.GetInt("csPointer",0);
        carPointer = PlayerPrefs.GetInt("carPointer",0);
        if(carPointer < listCarSets[carSetPointer].Cars.Length - 1)
        {
            invLeftButton.SetActive(true);
            if(carPointer < listCarSets[carSetPointer].Cars.Length - 2)
            {
                invRightButton.SetActive(true);
            }
            else
            {
                invRightButton.SetActive(false);
            }
            Destroy(GameObject.FindGameObjectWithTag("AI"));
            carPointer++;
            PlayerPrefs.SetInt("carPointer",carPointer);
            GameObject childObject = Instantiate(listCarSets[carSetPointer].Cars[carPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
            childObject.transform.parent = rotateTurnTable.transform;

            childObject.GetComponent<AICarControl>().enabled = false;
            childObject.GetComponent<AICarBehaviour>().enabled = false;
            InvCarInfo();
        }
    }

    public void InvLeftButtonClicked()
    {
        carSetPointer = PlayerPrefs.GetInt("csPointer",0);
        carPointer = PlayerPrefs.GetInt("carPointer",0);
        if(carPointer > 0)
        {
            invRightButton.SetActive(true);
            if(carPointer > 1)
            {
                invLeftButton.SetActive(true);
            }
            else 
            {
                invLeftButton.SetActive(false);
            }
            Destroy(GameObject.FindGameObjectWithTag("AI"));
            carPointer--;
            PlayerPrefs.SetInt("carPointer",carPointer);
            GameObject childObject = Instantiate(listCarSets[carSetPointer].Cars[carPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
            childObject.transform.parent = rotateTurnTable.transform;

            childObject.GetComponent<AICarControl>().enabled = false;
            childObject.GetComponent<AICarBehaviour>().enabled = false;
            InvCarInfo();
        }
    }

    public void InvCarInfo()
    {
        carSetPointer = PlayerPrefs.GetInt("csPointer",0);
        int carIndex = PlayerPrefs.GetInt("carPointer",0);
        GameObject car = listCarSets[carSetPointer].Cars[carIndex];
        AIController ac = car.GetComponent<AIController>();
        invCurrency.text = PlayerPrefs.GetInt("currency").ToString();

        string carIndexString = "";
        if (car.name.Length == 6)//one-digit number AI car has 6 characters (for example: AICar2)
        {
                carIndexString = car.name.Substring(car.name.Length - 1);
        }
        Debug.Log("carIndex" + carIndexString);
        if (PlayerPrefs.GetInt("Car_" + carIndexString) == 1)
        {
            carInvInfo.text = ac.carName + "    (Owned)";
        }
        else
        {
            carInvInfo.text = ac.carName + "    " + ac.carPrice;
        }
    }

    public void InvCarSet1ButtonClicked()
    {
        invLeftButton.SetActive(false);
        invRightButton.SetActive(true);
        carSetPointer = 0;
        PlayerPrefs.SetInt("csPointer",carSetPointer);
        carPointer = 0;
        PlayerPrefs.SetInt("carPointer",carPointer);
        int chooseIndex = PlayerPrefs.GetInt("chPointer");
        lockButton.SetActive(true);
        if(chooseIndex == carSetPointer)
        {
            lockImage.gameObject.SetActive(true);
            unlockImage.gameObject.SetActive(false);
        }
        else 
        {
            lockImage.gameObject.SetActive(false);
            unlockImage.gameObject.SetActive(true);
        }
        Destroy(GameObject.FindGameObjectWithTag("AI"));
        GameObject childObject = Instantiate(listCarSets[carSetPointer].Cars[carPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
        childObject.transform.parent = rotateTurnTable.transform;

        childObject.GetComponent<AICarControl>().enabled = false;
        childObject.GetComponent<AICarBehaviour>().enabled = false;
        InvCarInfo();
    }
    public void InvCarSet2ButtonClicked()
    {
        invLeftButton.SetActive(false);
        invRightButton.SetActive(true);
        carSetPointer = 1;
        PlayerPrefs.SetInt("csPointer",carSetPointer);
        carPointer = 0;
        PlayerPrefs.SetInt("carPointer",carPointer);
        int chooseIndex = PlayerPrefs.GetInt("chPointer");
        lockButton.SetActive(true);
        if(chooseIndex == carSetPointer)
        {
            lockImage.gameObject.SetActive(true);
            unlockImage.gameObject.SetActive(false);
        }
        else 
        {
            lockImage.gameObject.SetActive(false);
            unlockImage.gameObject.SetActive(true);
        }
        Destroy(GameObject.FindGameObjectWithTag("AI"));
        GameObject childObject = Instantiate(listCarSets[carSetPointer].Cars[carPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
        childObject.transform.parent = rotateTurnTable.transform;

        childObject.GetComponent<AICarControl>().enabled = false;
        childObject.GetComponent<AICarBehaviour>().enabled = false;
        InvCarInfo();
    }
    public void InvCarSet3ButtonClicked()
    {
        invLeftButton.SetActive(false);
        invRightButton.SetActive(true);
        carSetPointer = 2;
        PlayerPrefs.SetInt("csPointer",carSetPointer);
        carPointer = 0;
        PlayerPrefs.SetInt("carPointer",carPointer);
        int chooseIndex = PlayerPrefs.GetInt("chPointer");
        lockButton.SetActive(true);
        if(chooseIndex == carSetPointer)
        {
            lockImage.gameObject.SetActive(true);
            unlockImage.gameObject.SetActive(false);
        }
        else 
        {
            lockImage.gameObject.SetActive(false);
            unlockImage.gameObject.SetActive(true);
        }
        Destroy(GameObject.FindGameObjectWithTag("AI"));
        GameObject childObject = Instantiate(listCarSets[carSetPointer].Cars[carPointer],Vector3.zero,rotateTurnTable.transform.rotation) as GameObject;
        childObject.transform.parent = rotateTurnTable.transform;

        childObject.GetComponent<AICarControl>().enabled = false;
        childObject.GetComponent<AICarBehaviour>().enabled = false;
        InvCarInfo();
    }
    public void LockButtonClicked()
    {
        bool isUnlockActive = unlockImage.gameObject.activeSelf;
        int carSet = PlayerPrefs.GetInt("csPointer");
        bool isHasAllCar = CheckIfHaveAllCar(carSet);
        if(isHasAllCar)
        {
            if(isUnlockActive)
            {
                unlockImage.gameObject.SetActive(false);
                lockImage.gameObject.SetActive(true);
                PlayerPrefs.SetInt("chPointer" , carSetPointer);
                Debug.Log("chCarset:" + PlayerPrefs.GetInt("chPointer"));
            }
        }
        else
        {
            Debug.Log("You don't have all cars");
            ShowNotifMessage("You don't have all cars in this Set!");
            Invoke("CloseNotifMessage",1.5f);
        }
    }
    public void InvReturnButtonClicked()
    {
        invCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        mainMenu.SetActive(true);

        startToFinal = true;
        finalToStart = false;
        chCarSet = PlayerPrefs.GetInt("chPointer");
        PlayerPrefs.SetInt("csPointer",chCarSet);

        saveLoadManager.playerData.coin = PlayerPrefs.GetInt("currency");
        saveLoadManager.playerData.lastChoose = PlayerPrefs.GetInt("csPointer");
        SaveOwnedCarData();

        mainCurrency.text = saveLoadManager.playerData.coin.ToString();
        saveLoadManager.SaveData();
    }
    //Setting canvas
	void DisablePanels()
    {
		settingsCanvas.SetActive(false);
	}

	public void GameButtonClicked()
    {
		PanelControls.SetActive(false);
        lineControls.SetActive(false);
		PanelGame.SetActive(true);
		lineGame.SetActive(true);
	}

    public void NormalButtonClicked()
    {
        lineHardcore.SetActive(false);
        lineNormal.SetActive(true);
        isHard = 0;
        PlayerPrefs.SetInt("IsHard",isHard);
        Debug.Log("IsHard = " + isHard);
    }

    public void HardcoreButtonClicked()
    {
        lineNormal.SetActive(false);
        lineHardcore.SetActive(true);
        isHard = 1;
        PlayerPrefs.SetInt("IsHard",isHard);
        Debug.Log("IsHard = " + isHard);
    }

	public void ControlButtonClicked()
    {
		PanelGame.SetActive(false);
		lineGame.SetActive(false);
		PanelControls.SetActive(true);
		lineControls.SetActive(true);
	}
    
    //Exit canvas
    public void QuitGame()
    {
        saveLoadManager.playerData.coin = PlayerPrefs.GetInt("currency");
        saveLoadManager.playerData.lastChoose = PlayerPrefs.GetInt("csPointer");
        SaveOwnedCarData();
        saveLoadManager.SaveData();
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
			Application.Quit();
		#endif
	}

    public void PlayHover()
    {
		hoverSound.Play();
	}

	public void PlaySFXHover()
    {
		sliderSound.Play();
	}

	public void PlaySwoosh()
    {
		swooshSound.Play();
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
    public bool CheckIfHaveAllCar(int carSet)
    {
        bool hasCar = true;
        for(int i = 0; i < listCarSets[carSetPointer].Cars.Length; i++)
        {
            GameObject car = listCarSets[carSetPointer].Cars[i];
            AIController ac = car.GetComponent<AIController>();
            string carIndexString = "";
            if (car.name.Length == 6)//one-digit number AI car has 6 characters (for example: AICar2)
            {
                carIndexString = car.name.Substring(car.name.Length - 1);
            }
            Debug.Log("carIndex" + carIndexString);
            if (PlayerPrefs.GetInt("Car_" + carIndexString) != 1)
            {
                hasCar = false;
                break;
            }
        }
        return hasCar;
    }
}