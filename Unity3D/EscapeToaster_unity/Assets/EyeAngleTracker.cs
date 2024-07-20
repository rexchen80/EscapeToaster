using UnityEngine;
using System.IO.Ports;

public class EyeAngleTracker : MonoBehaviour
{
    public Transform centerEyeAnchor;
    public Transform trackingSpace;
    private SerialPort serialPort;
    private string portName = "COM4";
    private int baudRate = 115200;
    private float sendInterval = 1f / 30f; // 每秒30次，约0.0333秒
    private float timer = 0f;

    void Start()
    {
        // 初始化串口
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
                // 获取CenterEyeAnchor相对于TrackingSpace的局部旋转
                Quaternion relativeRotation = Quaternion.Inverse(trackingSpace.rotation) * centerEyeAnchor.rotation;
                // 将旋转转换为欧拉角
                Vector3 angles = relativeRotation.eulerAngles;

                // 调整俯仰角，使原点为-90度
                float pitch = angles.x + 90f;
                if (pitch > 360f)
                    pitch -= 360f;

                // 调整偏航角，使初始值为90度
                float yaw = 90f - angles.y;
                if (yaw < 0f)
                    yaw += 360f;
                if (yaw > 180f)
                    yaw = 360f - yaw;

                // 约束偏航角和俯仰角
                yaw = Mathf.Clamp(yaw, 0f, 180f);
                pitch = Mathf.Clamp(pitch, 20f, 150f);

                // 打印校准后的角度值到控制台
                Debug.Log($"Calibrated Pitch (俯仰角): {pitch:F2}°, Calibrated Yaw (偏航角): {yaw:F2}°");
                // 发送数据到串口
                SendDataToSerial(yaw, pitch);
            }
            else
            {
                Debug.LogWarning("CenterEyeAnchor 或 TrackingSpace 未设置!");
            }
            // 重置计时器
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