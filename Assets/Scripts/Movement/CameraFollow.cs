using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using UnityEngine.Assertions;

public class CameraFollow : NetworkBehaviour
{
    private CinemachineFreeLook m_MainCamera;

    void Start()
    {
        AttachCamera();
    }
    private void Update()
    {
        if (IsServer)
        {
            m_MainCamera.m_XAxis.Value = -180f;
        }
    }

    private void AttachCamera()
    {
        m_MainCamera = GameObject.FindObjectOfType<CinemachineFreeLook>();
        Assert.IsNotNull(m_MainCamera, "CameraController.AttachCamera: Couldn't find gameplay freelook camera");

        if (m_MainCamera)
        {
            // camera body / aim
            m_MainCamera.Follow = transform;
            m_MainCamera.LookAt = transform;
            // default rotation / zoom
            m_MainCamera.m_Heading.m_Bias = 40f;
            m_MainCamera.m_YAxis.Value = 0.5f;
            
            

        }

    }
   
}