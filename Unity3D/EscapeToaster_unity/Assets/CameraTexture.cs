using UnityEngine;

public class WebcamTextureApplier : MonoBehaviour
{
    public Material materialToApply; // ��ק�㴴���Ĳ��ʵ�����ֶ�
    private WebCamTexture webcamTexture;
    private WebCamDevice[] devices;

    void Start()
    {
        // ��ȡ����ͷ�豸�б�
        devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.LogError("No webcams found!");
            return;
        }

        // ѡ���һ������ͷ�豸
        webcamTexture = new WebCamTexture(devices[0].name);

        // ��WebCamTextureӦ�õ�����
        materialToApply.mainTexture = webcamTexture;

        // ����WebCamTexture
        webcamTexture.Play();
    }

    void OnDestroy()
    {
        // �ͷ�WebCamTexture��Դ
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }
    }
}