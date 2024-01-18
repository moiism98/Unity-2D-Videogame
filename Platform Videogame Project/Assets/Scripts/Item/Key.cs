using System;
using UnityEngine;

public class Key : MonoBehaviour, ItemInterface
{
    public static event Action<int, int> OnKeyCollect;
    private GameController gameController;
    private int keyValue = 1;
    [SerializeField] private int keyScore = 350;
    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }
    public void Collect()
    {
        OnKeyCollect.Invoke(keyValue, keyValue);

        gameController.ShowEarnedScore(keyScore, transform);

        Destroy(gameObject);
    }
}
