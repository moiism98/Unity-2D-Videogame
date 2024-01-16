using System;
using UnityEngine;

[Serializable]
public class Stage
{
    [SerializeField] private int index;
    [SerializeField] private GameObject stage;

    public int GetIndex()
    {
        return this.index;
    }

    public GameObject GetStage()
    {
        return this.stage;
    }
}
