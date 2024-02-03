using System;
using UnityEngine;

public class Key : MonoBehaviour, ItemInterface
{
    public static event Action<int, int> OnKeyCollect;
    private GameController gameController;
    private AudioManager audioManager;
    [SerializeField] private int keyValue = 1;
    [SerializeField] private int keyScore = 350;
    private void Start()
    {
        gameController = FindObjectOfType<GameController>();

        audioManager = FindObjectOfType<AudioManager>();
    }
    public void Collect()
    {
        OnKeyCollect.Invoke(keyValue, keyScore);

        gameController.ShowEarnedScore(keyScore, transform);

        Destroy(gameObject);

        audioManager.PlaySound("Key");
    }
}
