using System;
using UnityEngine;

public class Fruit : MonoBehaviour, ItemInterface
{
    public static event Action<int> OnFruitCollect;
    [SerializeField] private int fruitScore = 200;
    [SerializeField] private GameObject collectAnimation;
    private GameController gameController;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    public void Collect()
    {
        OnFruitCollect.Invoke(fruitScore);

        Destroy(gameObject);

        Instantiate(collectAnimation, transform.position, Quaternion.identity);

        gameController.ShowEarnedScore(fruitScore, transform);
    }
}
