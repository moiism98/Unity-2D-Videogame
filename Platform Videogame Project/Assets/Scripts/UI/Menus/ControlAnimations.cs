using UnityEngine;

public class ControlAnimations : MonoBehaviour
{
    private DeviceController deviceController;
    [SerializeField] private Animator animator;
    [SerializeField] private RuntimeAnimatorController[] animatorController;
    private void Start()
    {
        deviceController = FindObjectOfType<DeviceController>();
    }

    private void Update()
    {
        deviceController.ShowDeviceUI(animator, animatorController);
    }
}
