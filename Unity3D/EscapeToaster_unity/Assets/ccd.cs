using UnityEngine;
using RootMotion.FinalIK;

public class CCD : MonoBehaviour
{
    CCDIK ccdIK;

    // ��ȡccdIK
    void Start()
    {
        ccdIK = GetComponent<CCDIK>();
    }

    // ����ccdIK��������x ����Ϊ -10
    void LateUpdate()
    {
        if (ccdIK != null)
        {
            //ccdIK.solver.IKPosition.x = -10;
        }
    }
}