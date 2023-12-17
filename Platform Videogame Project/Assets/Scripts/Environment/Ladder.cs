using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private LadderAction ladderAction = LadderAction.notClimbable;
    [SerializeField] private GameObject buttonBubble;
    [SerializeField] private Animator ladderAnimator;
    [SerializeField] private AnimationClip[] animationClips;

    private void Start()
    {
        buttonBubble.SetActive(false);
    }
    private void OnTriggerEnter2D()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if(playerController != null)
        {
            // display climb button UI

            // buttonBubble.SetActive(true);

            ladderAction = LadderAction.climbable;
        }
    }

    private void OnTriggerExit2D()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if(playerController != null)
        {
            // hide climb button UI

            /*buttonBubble.SetActive(false);

            ladderAanimator.Rebind();*/

            ladderAction = LadderAction.notClimbable;
        }
    }
}
