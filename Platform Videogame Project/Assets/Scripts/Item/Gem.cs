using System;
using UnityEngine;

public class Gem : MonoBehaviour, ItemInterface
{
    private GameController gameController;
    public static event Action<int> OnGemCollect;
    [SerializeField] private int gemValue = 1000;
    [SerializeField] private GameObject collectAnimation;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }
    public void Collect()
    {
        OnGemCollect.Invoke(gemValue);

        Instantiate(collectAnimation, transform.position, Quaternion.identity);

        gameController.ShowEarnedScore(gemValue, transform);

        Destroy(gameObject);
    }
}
