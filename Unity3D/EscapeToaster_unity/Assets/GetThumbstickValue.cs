using UnityEngine;

public class OmniWheelDrive : MonoBehaviour
{
    private ServoController servoController;

    void Start()
    {
        // ��ȡServoController���
        servoController = GameObject.Find("ServoBusCtrl").GetComponent<ServoController>();
    }

    void Update()
    {
        // ��ȡ���ֲ��ݸ˵�ҡ����ֵ
        Vector2 leftThumbstickValue = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

        // ��ȡ���ֲ��ݸ˵�ҡ����ֵ
        Vector2 rightThumbstickValue = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.RTouch);

        // ����������ź�
        float vX = leftThumbstickValue.x; // ˮƽ�ƶ�
        float vY = leftThumbstickValue.y; // ��ֱ�ƶ�
        float vRotation = rightThumbstickValue.x;//ԭ����ת

        // ����ÿ��������ٶ�
        float motor5Speed = 0.5f * vX + 0.866f * vY + vRotation; // sin(120��) = 0.866, cos(120��) = -0.5
        float motor6Speed = 0.5f * vX - 0.866f * vY + vRotation; // sin(-120��) = -0.866, cos(-120��) = -0.5
        float motor7Speed = -vX + vRotation; // sin(180��) = 0, cos(180��) = -1

        // ����Χ��-1��1ת����-1000��1000
        motor5Speed *= 200;
        motor6Speed *= 200;
        motor7Speed *= 200;

        // ���Ƶ��������ź���-1000��1000֮��
        motor5Speed = Mathf.Clamp(motor5Speed, -200, 200);
        motor6Speed = Mathf.Clamp(motor6Speed, -200, 200);
        motor7Speed = Mathf.Clamp(motor7Speed, -200, 200);

        // ���ź�д�뵽ServoController
        servoController.servo5.WheelSpeed = (int)motor5Speed;
        servoController.servo6.WheelSpeed = (int)motor6Speed;
        servoController.servo7.WheelSpeed = (int)motor7Speed;

        // �������ٶ�
        //Debug.Log($"Motor5 Speed: {motor5Speed}, Motor6 Speed: {motor6Speed}, Motor7 Speed: {motor7Speed}");
    }
}
