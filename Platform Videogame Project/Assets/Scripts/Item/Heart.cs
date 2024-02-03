using System;
using UnityEngine;

public class Heart : MonoBehaviour, ItemInterface
{
    public static event Action<int> OnHeartCollect;
    private int heartValue = 1;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    public void Collect()
    {
        Destroy(gameObject);

        audioManager.PlaySound("Heart, fruit and arrow");

        OnHeartCollect.Invoke(heartValue);
    }
}
