using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class DeviceController : MonoBehaviour
{
    private PlayerInput playerInput;
    public static string deviceInUse;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    void Update()
    {   
        DetectDeviceInUse();

        Debug.Log(playerInput.currentActionMap);
    }

    private void DetectDeviceInUse()
    {
        foreach(InputDevice device in playerInput.devices)
           deviceInUse = device.displayName;
    }

    public void UpdateGameActions(string actionMap)
    {
        playerInput.SwitchCurrentActionMap(actionMap);
    }

    public void ShowDeviceUI(Animator animator, RuntimeAnimatorController[] animatorControllers)
    {
        switch(deviceInUse)
        {
            case "Keyboard": GetAnimatorController("keyboard", animator, animatorControllers); break;

            case "Xbox Controller": GetAnimatorController("xbox", animator, animatorControllers); break;

            default: GetAnimatorController("ps", animator, animatorControllers); break;
        }
    }

    private void GetAnimatorController(string controllerName, Animator animator, RuntimeAnimatorController[] animatorControllers)
    {
        RuntimeAnimatorController animatorController = Array.Find(animatorControllers, controller => controller.name.Contains(controllerName));

        animator.runtimeAnimatorController = animatorController;
    }
}
