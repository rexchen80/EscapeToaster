using UnityEngine;
using RootMotion.FinalIK;

public class CCD : MonoBehaviour
{
    CCDIK ccdIK;

    // 获取ccdIK
    void Start()
    {
        ccdIK = GetComponent<CCDIK>();
    }

    // 设置ccdIK解算器的x 坐标为 -10
    void LateUpdate()
    {
        if (ccdIK != null)
        {
            //ccdIK.solver.IKPosition.x = -10;
        }
    }
}