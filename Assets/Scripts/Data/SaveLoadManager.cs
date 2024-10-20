using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.SceneManagement;
public class SaveLoadManager : MonoBehaviour
{
    public PlayerData playerData;
    
    void Awake()
    {
        // Đảm bảo đối tượng này không bị hủy khi chuyển scene
        DontDestroyOnLoad(this.gameObject);
    }
    //savedata
    public void SaveData()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"Coins", playerData.coin.ToString()},
                {"LastChoose", playerData.lastChoose.ToString()},
                {"OwnedCars", ConvertIntArrayToString(playerData.OwnedCar)},
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnUpdateUserDataSuccess, OnUpdateUserDataFailure);
    }

    private void OnUpdateUserDataFailure(PlayFabError error)
    {
        Debug.LogError("Update user data failed: " + error.ErrorMessage);
    }

    private void OnUpdateUserDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log("User data saved successfully!");
    }

    private string ConvertIntArrayToString(int[] array)
    {
        return string.Join(",", array);
    }

    //loaddata
    public void LoadData()
    {
        // Tạo một yêu cầu lấy dữ liệu người dùng từ PlayFab
        var request = new GetUserDataRequest();

        // Gọi API của PlayFab để lấy dữ liệu người dùng
        PlayFabClientAPI.GetUserData(request, OnGetUserDataSuccess, OnGetUserDataFailure);
    }

    // Xử lý khi lấy dữ liệu thành công
    private void OnGetUserDataSuccess(GetUserDataResult result)
    {
        Debug.Log("User data loaded successfully!");

        // Kiểm tra và gán dữ liệu từ kết quả trả về
        if (result.Data.TryGetValue("Coins", out UserDataRecord coinData))
        {
            playerData.coin = int.Parse(coinData.Value);
        }

        if (result.Data.TryGetValue("LastChoose", out UserDataRecord lastChooseData))
        {
            playerData.lastChoose = int.Parse(lastChooseData.Value);
        }

        if (result.Data.TryGetValue("OwnedCars", out UserDataRecord ownedCarsData))
        {
            playerData.OwnedCar = ConvertStringToIntArray(ownedCarsData.Value);
        }
    }

    // Xử lý khi lấy dữ liệu thất bại
    private void OnGetUserDataFailure(PlayFabError error)
    {
        Debug.LogError("Get user data failed: " + error.ErrorMessage);
        // Xử lý sau khi lấy dữ liệu thất bại (nếu cần)
    }

    // Phương thức chuyển đổi chuỗi thành mảng số nguyên
    private int[] ConvertStringToIntArray(string str)
    {
        string[] strArray = str.Split(',');
        int[] intArray = new int[strArray.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            intArray[i] = int.Parse(strArray[i]);
        }
        return intArray;
    }
}
