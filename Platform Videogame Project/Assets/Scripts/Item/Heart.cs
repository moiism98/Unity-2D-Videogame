using System;
using UnityEngine;

public class Heart : MonoBehaviour, ItemInterface
{
    public static event Action<int> OnHeartCollect;
    private int heartValue = 1;
    public void Collect()
    {
        Destroy(gameObject);

        OnHeartCollect.Invoke(heartValue);
    }
}
