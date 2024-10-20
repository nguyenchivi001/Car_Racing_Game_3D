using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.SceneManagement;

public class PlayfabManager : MonoBehaviour
{
    public SaveLoadManager saveLoadManager;
    public GameObject loginPanel, signupPanel, recoverPanel, notificationsPanel;
    public TMP_InputField loginEmail, loginPassword;
    public TMP_InputField signupUsername, signupEmail, signupPassword, signupCPassword;
    public TMP_InputField recoverEmail;
    public TextMeshProUGUI notif_title, notif_message;

    void Start()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        recoverPanel.SetActive(false);
        notificationsPanel.SetActive(false);

        saveLoadManager = FindObjectOfType<SaveLoadManager>();

        if (saveLoadManager == null)
        {
            Debug.LogError("Không tìm thấy đối tượng SaveLoadManager trong Scene!");
        }
    }
    public void OpenLoginScreen()
    {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        recoverPanel.SetActive(false);
    }
    public void OpenSignupScreen()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        recoverPanel.SetActive(false);
    }
    public void OpenRecoverScreen()
    {
        loginPanel.SetActive(false);
        recoverPanel.SetActive(true);
        signupPanel.SetActive(false);
    }

    //login
    public void LoginUser()
    {
        // do login
        var request = new LoginWithEmailAddressRequest
        {
            Email = loginEmail.text,
            Password = loginPassword.text,
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }
    
    //recover
    public void RecoverUser()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = recoverEmail.text,
            TitleId = "6461A",
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverSuccess, OnError);
    }

    //signup
    public void SignupUser()
    {
        //do sign up
        var request = new RegisterPlayFabUserRequest
        {
            Username = signupUsername.text,
            Password = signupPassword.text,
            Email = signupEmail.text,

            RequireBothUsernameAndEmail = true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSucces, OnError);
    }

    private void OnError(PlayFab.PlayFabError Error)
    {
        ShowNotificationMessage("Error",Error.ErrorMessage);
        Invoke("CloseNotificationMessage",2f);
    }
    private void OnLoginSuccess(PlayFab.ClientModels.LoginResult result)
    {
        ShowNotificationMessage("Success","login in");
        Invoke("CloseNotificationMessage", 1f);
        Invoke("GoToMainScene",2f);
        saveLoadManager.LoadData();
        PlayerPrefs.DeleteAll();
    }
    private void OnRecoverSuccess(SendAccountRecoveryEmailResult result)
    {
        OpenLoginScreen();
        ShowNotificationMessage("Success", "An email has been sent to " + recoverEmail.text );
        Invoke("CloseNotificationMessage", 1f);
    }
    private void OnRegisterSucces(RegisterPlayFabUserResult Result)
    {
        ShowNotificationMessage("Success","New account is already registered");
        Invoke("CloseNotificationMessage", 1f);
        Invoke("GoToMainScene",2f);
        saveLoadManager.SaveData();
        PlayerPrefs.DeleteAll();
    }

    //Show notification
    private void ShowNotificationMessage(string title,string message)
    {
        notif_title.text = "" + title;
        notif_message.text = "" + message;
        notificationsPanel.SetActive(true);
    }
    public void CloseNotificationMessage()
    {
        notif_title.text = "";
        notif_message.text = "";
        notificationsPanel.SetActive(false);
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
