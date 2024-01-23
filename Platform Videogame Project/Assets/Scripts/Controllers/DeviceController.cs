using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class DeviceController : MonoBehaviour
{
    private GameController gameController;
    private PlayerController playerController;
    private PlayerInput playerInput;
    public static string deviceInUse;

    [Header("Item Actions")]
    private Lever lever;
    void Start()
    {
        gameController = FindObjectOfType<GameController>();

        playerController = FindObjectOfType<PlayerController>();

        playerInput = GetComponent<PlayerInput>();
    }
    void Update()
    {   
        DetectDeviceInUse();

        //Debug.Log(playerInput.currentActionMap);

        // we have to store the player controller whenever it's value is null because we need to use it's actions as jump, run, walk and so on.
        // when the player dies, we disapear it and this also happens to the Player Controller script.

        if(playerController != null) 
            playerController = FindObjectOfType<PlayerController>();
    }

    /// <summary>
    /// Method which is going to trigger the player input events using the Player Controller script where the actions are stored.
    /// </summary>
    /// <param name="ctx"></param> <summary>
    /// 
    /// </summary>
    /// <param name="ctx"></param>
    public void SetEvent(InputAction.CallbackContext ctx)
    {
        switch (ctx.action.name)
        {
           case "Walk": playerController.Walk(ctx); break;
           case "Run": playerController.Run(ctx); break;
           case "Jump": playerController.Jump(ctx); break;
           case "Crouch": playerController.Crouch(ctx); break;
           case "Climb": playerController.Climb(ctx); break;
           case "PassThroughPlatform": playerController.PassThroughPlatform(ctx); break;
           case "Shoot": playerController.Shoot(ctx); break;
        }
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

    #region Game Actions
        public void Action(InputAction.CallbackContext action)
        {
            if(action.performed)
            {
                switch(gameController.GetGameAction())
                {
                    case GameAction.exit: gameController.LoadNextLevel(); break;

                    case GameAction.lever: lever.Activate(); break;

                    case GameAction.climb:

                        if(Ladder.isClimbable)
                        {
                            // if we pressed the climb action button while climbing will stop the climb action again!

                            if(playerController.GetIsClimbing())
                                playerController.SetIsClimbing(!playerController.GetIsClimbing());

                            Ladder ladder = gameController.GetLadder();

                            if(ladder != null && ladder.GetIsClimbable())
                            {
                                // the player starts climbing with no moving

                                Rigidbody2D rb = playerController.GetRigidbody2D();

                                rb.velocity = Vector2.zero;

                                playerController.SetMovement(0f);

                                // player starts climbing at the climb point

                                playerController.gameObject.transform.position = new Vector2(ladder.transform.position.x, ladder.transform.position.y);

                                playerController.SetIsClimbing(!playerController.GetIsClimbing());

                                rb.gravityScale = 0; // we cancel the gravity on climb so we can not slide over the ladder

                                ladder.HideControllerButton(); // and hide the button bubble

                                // the players body colliders are disable while it's climbing

                                playerController.DisablePlayerCols();
                            }
                        }

                    break;
                }
            }
        }

    #endregion

    #region Getter/Setter
        public void SetLever(Lever lever)
        {
            this.lever = lever;
        }

    #endregion
}
