using Unity.VisualScripting;
using UnityEngine;

public class ArrowItem : MonoBehaviour, ItemInterface
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    public void Collect()
    {
        Destroy(gameObject);

        audioManager.PlaySound("Heart, fruit and arrow");

        GameController.isArrowReady = true;
    }
}
