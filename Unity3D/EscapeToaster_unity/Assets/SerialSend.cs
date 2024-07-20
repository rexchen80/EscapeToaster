using UnityEngine;
using System.IO.Ports;
using System;
using System.Collections;

public class ServoController : MonoBehaviour
{
    private SerialPort serialPort;
    private const float SendInterval = 1f / 60f; // 60Hz ����Ƶ��
    private const float FullDataSendInterval = 1f / 30f; // ÿ�뷢��һ����������
    private float lastFullDataSendTime = 0f;

    [SerializeField] private string portName = "COM10";

    [System.Serializable]
    public class ServoParams
    {
        [Range(0, 360)] public float angle = 180f;
        [Range(0, 1023)] public int maxTorque = 350;
    }

    [System.Serializable]
    public class WheelParams
    {
        [Range(-1000, 1000)] public int WheelSpeed = 0;
    }

    [Header("Servos")]
    public ServoParams servo1 = new ServoParams();
    public ServoParams servo2 = new ServoParams();
    public ServoParams servo3 = new ServoParams();
    public ServoParams servo4 = new ServoParams();
    public ServoParams servo8 = new ServoParams();
    public ServoParams servo9 = new ServoParams();
    public ServoParams servo10 = new ServoParams();
    public ServoParams servo11 = new ServoParams();

    [Header("Wheels")]
    public WheelParams servo5 = new WheelParams();
    public WheelParams servo6 = new WheelParams();
    public WheelParams servo7 = new WheelParams();

    private ServoParams[] servos;
    private WheelParams[] wheels;

    private float[] lastSentAngles = new float[8];
    private int[] lastSentMaxTorque = new int[8];
    private int[] lastSentWheelSpeeds = new int[3];

    private void Awake()
    {
        servos = new ServoParams[] { servo1, servo2, servo3, servo4, servo8, servo9, servo10, servo11 };
        wheels = new WheelParams[] { servo5, servo6, servo7 };
    }

    private void Start()
    {
        InitializeLastSentValues();
        OpenSerialPort();
    }

    private void InitializeLastSentValues()
    {
        for (int i = 0; i < 8; i++)
        {
            lastSentAngles[i] = -1f;
            lastSentMaxTorque[i] = -1;
        }
        for (int i = 0; i < 3; i++)
        {
            lastSentWheelSpeeds[i] = 1001; // ��ʼ��Ϊ��Χ֮���ֵ
        }
    }

    private void OpenSerialPort()
    {
        try
        {
            serialPort = new SerialPort(portName, 1000000, Parity.None, 8, StopBits.One);
            serialPort.Open();
            Debug.Log("�����Ѵ�");
            StartCoroutine(SendDataRoutine());
        }
        catch (Exception e)
        {
            Debug.LogError("���ڴ򿪴���: " + e.Message);
        }
    }

    private IEnumerator SendDataRoutine()
    {
        while (true)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                float currentTime = Time.time;
                if (currentTime - lastFullDataSendTime >= FullDataSendInterval)
                {
                    SendAllServoData();
                    lastFullDataSendTime = currentTime;
                }

                bool needAction = CheckAndSendChangedData();
                if (needAction)
                {
                    SendAction();
                }
            }
            yield return new WaitForSeconds(SendInterval);
        }
    }

    private void SendAllServoData()
    {
        for (int i = 0; i < 8; i++)
        {
            int servoId = (i < 4) ? i + 1 : i + 4; // ��Ӧ������servo8-11
            SendRegWriteAngle(servoId, servos[i].angle);
            SendRegWriteMaxTorque(servoId, servos[i].maxTorque);
        }
        for (int i = 0; i < 3; i++)
        {
            SendRegWriteWheelSpeed(i + 5, wheels[i].WheelSpeed);
        }
        SendAction();
    }

    private bool CheckAndSendChangedData()
    {
        bool needAction = false;
        for (int i = 0; i < 8; i++)
        {
            int servoId = (i < 4) ? i + 1 : i + 4; // ��Ӧ������servo8-11
            needAction |= CheckAndSendServoData(servoId, servos[i]);
        }
        for (int i = 0; i < 3; i++)
        {
            needAction |= CheckAndSendWheelData(i + 5, wheels[i]);
        }
        return needAction;
    }

    private bool CheckAndSendServoData(int id, ServoParams servo)
    {
        bool needAction = false;
        int index = (id <= 4) ? id - 1 : id - 5; // ������������Ӧ�µ�servo����
        if (servo.angle != lastSentAngles[index])
        {
            SendRegWriteAngle(id, servo.angle);
            lastSentAngles[index] = servo.angle;
            needAction = true;
        }
        if (servo.maxTorque != lastSentMaxTorque[index])
        {
            SendRegWriteMaxTorque(id, servo.maxTorque);
            lastSentMaxTorque[index] = servo.maxTorque;
            needAction = true;
        }
        return needAction;
    }

    private bool CheckAndSendWheelData(int id, WheelParams wheel)
    {
        if (wheel.WheelSpeed != lastSentWheelSpeeds[id - 5])
        {
            SendRegWriteWheelSpeed(id, wheel.WheelSpeed);
            lastSentWheelSpeeds[id - 5] = wheel.WheelSpeed;
            return true;
        }
        return false;
    }

    private void SendRegWriteAngle(int id, float angle)
    {
        int position = Mathf.RoundToInt((angle / 360f) * 1023f);
        position = Mathf.Clamp(position, 0, 1023);

        byte[] command = new byte[9];
        command[0] = 0xFF;
        command[1] = 0xFF;
        command[2] = (byte)id;
        command[3] = 0x05;
        command[4] = 0x04; // REG WRITE instruction
        command[5] = 0x2A; // Starting address for Goal Position
        command[6] = (byte)(position >> 8);
        command[7] = (byte)(position & 0xFF);
        command[8] = CalculateChecksum(command);

        SendCommand(command, $"ID {id} REG WRITE �Ƕ�: {angle}��, λ��: {position}");
    }

    private void SendRegWriteMaxTorque(int id, int maxTorque)
    {
        int torque = Mathf.Clamp(maxTorque, 0, 1023);

        byte[] command = new byte[9];
        command[0] = 0xFF;
        command[1] = 0xFF;
        command[2] = (byte)id;
        command[3] = 0x05;
        command[4] = 0x04; // REG WRITE instruction
        command[5] = 0x1E; // Starting address for Max Torque
        command[6] = (byte)(torque >> 8);
        command[7] = (byte)(torque & 0xFF);
        command[8] = CalculateChecksum(command);

        SendCommand(command, $"ID {id} REG WRITE ���Ť��: {maxTorque}");
    }

    private void SendRegWriteWheelSpeed(int id, int wheelSpeed)
    {
        uint runtime;
        if (wheelSpeed < 0)
        {
            // ��ֵ��ʾ��ת��ӳ�䵽1025-2000��Χ
            runtime = (uint)(1025 + Math.Min(Math.Abs(wheelSpeed), 975));
        }
        else if (wheelSpeed > 1024)
        {
            // ����1024��ֵ��Ϊ��Ч������Ϊֹͣ
            runtime = 0;
        }
        else
        {
            // 0-1024��Χ�ڵ���ֱֵ��ʹ��
            runtime = (uint)wheelSpeed;
        }

        // ȷ��runtime������2000
        runtime = Math.Min(runtime, 2000);

        byte[] command = new byte[9];
        command[0] = 0xFF;
        command[1] = 0xFF;
        command[2] = (byte)id;
        command[3] = 0x05;
        command[4] = 0x04; // REG WRITE instruction
        command[5] = 0x2C; // Starting address for Runtime
        command[6] = (byte)(runtime >> 8);
        command[7] = (byte)(runtime & 0xFF);
        command[8] = CalculateChecksum(command);

        SendCommand(command, $"ID {id} REG WRITE ����: {wheelSpeed}, ����ʱ��: {runtime}");
    }

    private void SendAction()
    {
        byte[] command = new byte[6];
        command[0] = 0xFF;
        command[1] = 0xFF;
        command[2] = 0xFE; // Broadcast ID
        command[3] = 0x02;
        command[4] = 0x05; // ACTION instruction
        command[5] = CalculateChecksum(command);

        SendCommand(command, "ACTION");
    }

    private void SendCommand(byte[] command, string commandType)
    {
        try
        {
            serialPort.Write(command, 0, command.Length);
            Debug.Log($"����{commandType}����");
        }
        catch (Exception e)
        {
            Debug.LogError($"{commandType}���ݷ��ʹ���: " + e.Message);
        }
    }

    private byte CalculateChecksum(byte[] data)
    {
        uint sum = 0;
        for (int i = 2; i < data.Length - 1; i++)
        {
            sum += data[i];
        }
        return (byte)(~sum & 0xFF);
    }

    private void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                serialPort.Close();
                Debug.Log("�����ѹر�");
            }
            catch (Exception e)
            {
                Debug.LogError("���ڹرմ���: " + e.Message);
            }
        }
    }
}