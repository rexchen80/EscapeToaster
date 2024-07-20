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
                Debug.LogError("ServoBusCtrl������û���ҵ�ServoController�����");
            }
        }
        else
        {
            Debug.LogError("����Inspector��ָ��ServoBusCtrl���壡");
        }
    }

    void Update()
    {
        if (shoulderJoint != null && leftHandAnchor != null && servoBusCtrl != null)
        {
            // 1. ִ��LookRotation��ʹx�ᳯ��Ŀ��
            Vector3 direction = leftHandAnchor.position - shoulderJoint.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, -90, 0);
            shoulderJoint.rotation = lookRotation;

            // 2. ��ȡshoulder joint��ŷ����
            Vector3 rotation = shoulderJoint.localEulerAngles;

            // 3. ����ƫ����(Y����ת)��servo1����ȷ����0-360�ȷ�Χ��
            float yaw = (rotation.z +90+ 360) % 360;
            servoBusCtrl.servo1.angle = yaw;

            // 4. ���ø�����(Z����ת)��servo2���ȼ�90�ȣ�Ȼ��ȷ����0-360�ȷ�Χ��
            float pitch = (rotation.y +  180+ 360) % 360;
            servoBusCtrl.servo2.angle = pitch;

            // ����Ƕ���Ϣ���ڵ���
            //Debug.Log($"Yaw: {yaw}, Pitch: {pitch}");
        }
    }
}