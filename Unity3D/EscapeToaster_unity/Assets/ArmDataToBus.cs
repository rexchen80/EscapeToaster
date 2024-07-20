using UnityEngine;

public class RotationSender : MonoBehaviour
{
    public GameObject shoulderConnector;
    public GameObject upperArmConnector;
    public GameObject upperArmBody;
    public GameObject forearmBody;
    public ServoController servoController;

    void Update()
    {
        if (servoController != null)
        {
            // ��ȡ�粿�������ľֲ�Z����ת
            float shoulderAngle = NormalizeAngle(shoulderConnector.transform.localEulerAngles.z + 180);
            servoController.servo8.angle = shoulderAngle;

            // ��ȡ�ϱ��������ľֲ�X����ת
            float upperArmConnectorAngle = NormalizeAngle(360 - upperArmConnector.transform.localEulerAngles.x + 180);
            servoController.servo9.angle = upperArmConnectorAngle;
            // ��ȡ�ϱ۱���ľֲ�Y����ת
            float upperArmBodyAngle = NormalizeAngle(360 -  upperArmBody.transform.localEulerAngles.y + 180);
            servoController.servo10.angle = upperArmBodyAngle;

            // ��ȡС�۱���ľֲ�Z����ת
            float forearmBodyAngle = NormalizeAngle(360 - forearmBody.transform.localEulerAngles.z + 180);
            servoController.servo11.angle = forearmBodyAngle;
        }
    }

    // ���Ƕȹ�һ����0-360�ȷ�Χ
    private float NormalizeAngle(float angle)
    {
        while (angle < 0)
        {
            angle += 360;
        }
        return angle % 360;
    }
}