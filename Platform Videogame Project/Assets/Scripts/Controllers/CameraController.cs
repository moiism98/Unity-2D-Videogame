using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera vCamera;
    private CinemachineConfiner confiner;

    private void Start()
    {
        vCamera = GetComponent<CinemachineVirtualCamera>();

        confiner = GetComponent<CinemachineConfiner>();
    }

    private void Update()
    {
        vCamera.Follow = GameObject.FindGameObjectWithTag("Player").transform;

        confiner.m_BoundingShape2D = GameObject.FindGameObjectWithTag("Level").GetComponent<PolygonCollider2D>();
    }
}
