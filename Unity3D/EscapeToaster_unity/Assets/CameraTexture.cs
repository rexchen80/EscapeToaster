using UnityEngine;

public class WebcamTextureApplier : MonoBehaviour
{
    public Material materialToApply; // 拖拽你创建的材质到这个字段
    private WebCamTexture webcamTexture;
    private WebCamDevice[] devices;

    void Start()
    {
        // 获取摄像头设备列表
        devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("No webcams found!");
            return;
        }

        // 选择第一个摄像头设备
        webcamTexture = new WebCamTexture(devices[0].name);

        // 将WebCamTexture应用到材质
        materialToApply.mainTexture = webcamTexture;

        // 播放WebCamTexture
        webcamTexture.Play();
    }

    void OnDestroy()
    {
        // 释放WebCamTexture资源
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }
    }
}