using UnityEngine;

public class ShoulderJointController : MonoBehaviour
{
    public Transform shoulderJoint;
    public Transform leftHandAnchor;
    public GameObject servoBusCtrlObject;
    private ServoController servoBusCtrl;

    void Start()
    {
        if (servoBusCtrlObject != null)
        {
            servoBusCtrl = servoBusCtrlObject.GetComponent<ServoController>();
            if (servoBusCtrl == null)
            {
                Debug.LogError("ServoBusCtrl物体上没有找到ServoController组件！");
            }
        }
        else
        {
            Debug.LogError("请在Inspector中指定ServoBusCtrl物体！");
        }
    }

    void Update()
    {
        if (shoulderJoint != null && leftHandAnchor != null && servoBusCtrl != null)
        {
            // 1. 执行LookRotation，使x轴朝向目标
            Vector3 direction = leftHandAnchor.position - shoulderJoint.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, -90, 0);
            shoulderJoint.rotation = lookRotation;

            // 2. 获取shoulder joint的欧拉角
            Vector3 rotation = shoulderJoint.localEulerAngles;

            // 3. 设置偏航角(Y轴旋转)到servo1，并确保在0-360度范围内
            float yaw = (rotation.z +90+ 360) % 360;
            servoBusCtrl.servo1.angle = yaw;

            // 4. 设置俯仰角(Z轴旋转)到servo2，先加90度，然后确保在0-360度范围内
            float pitch = (rotation.y +  180+ 360) % 360;
            servoBusCtrl.servo2.angle = pitch;

            // 输出角度信息用于调试
            //Debug.Log($"Yaw: {yaw}, Pitch: {pitch}");
        }
    }
}