using System;
using TMPro;
using UnityEngine;

public class Gem : MonoBehaviour, ItemInterface
{
    private GameController gameController;
    public static event Action<int> OnGemCollect;
    [SerializeField] private int gemValue = 1000;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }
    public void Collect()
    {
        OnGemCollect.Invoke(gemValue);

        Destroy(gameObject);

        gameController.ShowEarnedScore(gemValue, transform);
    }
}
