using System;
using UnityEngine;

public class Key : MonoBehaviour, ItemInterface
{
    public static event Action<int> OnKeyCollect;
    private int keyValue = 1;
    public void Collect()
    {
        OnKeyCollect.Invoke(keyValue);

        Destroy(gameObject);
    }
}
