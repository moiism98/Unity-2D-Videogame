using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowItem : MonoBehaviour, ItemInterface
{
    public void Collect()
    {
        Destroy(gameObject);

        GameController.isArrowReady = true;
    }
}
