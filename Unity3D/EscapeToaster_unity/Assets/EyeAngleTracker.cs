using UnityEngine;
using System.IO.Ports;

public class EyeAngleTracker : MonoBehaviour
{
    public Transform centerEyeAnchor;
    public Transform trackingSpace;
    private SerialPort serialPort;
    private string portName = "COM4";
    private int baudRate = 115200;
    private float sendInterval = 1f / 30f; // ÿ��30�Σ�Լ0.0333��
    private float timer = 0f;

    void Start()
    {
        // ��ʼ������
        serialPort = new System.IO.Ports.SerialPort(portName, baudRate);
        try
        {
            serialPort.Open();
            Debug.Log("Serial port opened successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error opening serial port: " + e.Message);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= sendInterval)
        {
            if (centerEyeAnchor != null && trackingSpace != null)
            {
                // ��ȡCenterEyeAnchor�����TrackingSpace�ľֲ���ת
                Quaternion relativeRotation = Quaternion.Inverse(trackingSpace.rotation) * centerEyeAnchor.rotation;
                // ����תת��Ϊŷ����
                Vector3 angles = relativeRotation.eulerAngles;

                // ���������ǣ�ʹԭ��Ϊ-90��
                float pitch = angles.x + 90f;
                if (pitch > 360f)
                    pitch -= 360f;

                // ����ƫ���ǣ�ʹ��ʼֵΪ90��
                float yaw = 90f - angles.y;
                if (yaw < 0f)
                    yaw += 360f;
                if (yaw > 180f)
                    yaw = 360f - yaw;

                // Լ��ƫ���Ǻ͸�����
                yaw = Mathf.Clamp(yaw, 0f, 180f);
                pitch = Mathf.Clamp(pitch, 20f, 150f);

                // ��ӡУ׼��ĽǶ�ֵ������̨
                Debug.Log($"Calibrated Pitch (������): {pitch:F2}��, Calibrated Yaw (ƫ����): {yaw:F2}��");
                // �������ݵ�����
                SendDataToSerial(yaw, pitch);
            }
            else
            {
                Debug.LogWarning("CenterEyeAnchor �� TrackingSpace δ����!");
            }
            // ���ü�ʱ��
            timer = 0f;
        }
    }

    void SendDataToSerial(float yaw, float pitch)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            string dataToSend = $"<servo>,{yaw:F2},{pitch:F2};";
            try
            {
                serialPort.WriteLine(dataToSend);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error sending data to serial port: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        CloseSerialPort();
    }

    void OnDisable()
    {
        CloseSerialPort();
    }

    void CloseSerialPort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Serial port closed");
        }
    }
}