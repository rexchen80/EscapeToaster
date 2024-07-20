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
            // 获取肩部连接器的局部Z轴旋转
            float shoulderAngle = NormalizeAngle(shoulderConnector.transform.localEulerAngles.z + 180);
            servoController.servo8.angle = shoulderAngle;

            // 获取上臂连接器的局部X轴旋转
            float upperArmConnectorAngle = NormalizeAngle(360 - upperArmConnector.transform.localEulerAngles.x + 180);
            servoController.servo9.angle = upperArmConnectorAngle;
            // 获取上臂本体的局部Y轴旋转
            float upperArmBodyAngle = NormalizeAngle(360 -  upperArmBody.transform.localEulerAngles.y + 180);
            servoController.servo10.angle = upperArmBodyAngle;

            // 获取小臂本体的局部Z轴旋转
            float forearmBodyAngle = NormalizeAngle(360 - forearmBody.transform.localEulerAngles.z + 180);
            servoController.servo11.angle = forearmBodyAngle;
        }
    }

    // 将角度归一化到0-360度范围
    private float NormalizeAngle(float angle)
    {
        while (angle < 0)
        {
            angle += 360;
        }
        return angle % 360;
    }
}