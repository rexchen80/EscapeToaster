using UnityEngine;

public class OmniWheelDrive : MonoBehaviour
{
    private ServoController servoController;

    void Start()
    {
        // 获取ServoController组件
        servoController = GameObject.Find("ServoBusCtrl").GetComponent<ServoController>();
    }

    void Update()
    {
        // 获取左手操纵杆的摇杆数值
        Vector2 leftThumbstickValue = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

        // 获取右手操纵杆的摇杆数值
        Vector2 rightThumbstickValue = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, OVRInput.Controller.RTouch);

        // 计算电机输出信号
        float vX = leftThumbstickValue.x; // 水平移动
        float vY = leftThumbstickValue.y; // 垂直移动
        float vRotation = rightThumbstickValue.x;//原地旋转

        // 计算每个电机的速度
        float motor5Speed = 0.5f * vX + 0.866f * vY + vRotation; // sin(120度) = 0.866, cos(120度) = -0.5
        float motor6Speed = 0.5f * vX - 0.866f * vY + vRotation; // sin(-120度) = -0.866, cos(-120度) = -0.5
        float motor7Speed = -vX + vRotation; // sin(180度) = 0, cos(180度) = -1

        // 将范围从-1到1转换到-1000到1000
        motor5Speed *= 200;
        motor6Speed *= 200;
        motor7Speed *= 200;

        // 限制电机的输出信号在-1000到1000之间
        motor5Speed = Mathf.Clamp(motor5Speed, -200, 200);
        motor6Speed = Mathf.Clamp(motor6Speed, -200, 200);
        motor7Speed = Mathf.Clamp(motor7Speed, -200, 200);

        // 将信号写入到ServoController
        servoController.servo5.WheelSpeed = (int)motor5Speed;
        servoController.servo6.WheelSpeed = (int)motor6Speed;
        servoController.servo7.WheelSpeed = (int)motor7Speed;

        // 输出电机速度
        //Debug.Log($"Motor5 Speed: {motor5Speed}, Motor6 Speed: {motor6Speed}, Motor7 Speed: {motor7Speed}");
    }
}
