using System;
using UnityEngine;

public class Gem : MonoBehaviour, ItemInterface
{
    public static event Action<int> OnGemCollect;
    [SerializeField] private int gemValue = 1000;
    public void Collect()
    {
        OnGemCollect.Invoke(gemValue);

        Destroy(gameObject);
    }
}
