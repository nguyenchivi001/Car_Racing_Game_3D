using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider;
    private AudioSource audioSource;

    void Start()
    {
        GameObject audioObject = GameObject.Find("PlayerDataManager");
        audioSource = audioObject.GetComponent<AudioSource>();
        // Thiết lập giá trị mặc định của Slider từ âm lượng hiện tại của Audio Source
        volumeSlider.value = audioSource.volume;

        // Đăng ký hàm UpdateVolume khi giá trị của Slider thay đổi
        volumeSlider.onValueChanged.AddListener(delegate { UpdateVolume(); });
    }

    void UpdateVolume()
    {
        // Cập nhật âm lượng của Audio Source dựa trên giá trị của Slider
        audioSource.volume = volumeSlider.value;
    }
}
