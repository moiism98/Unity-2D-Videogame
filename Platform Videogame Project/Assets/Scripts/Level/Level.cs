using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Level
{
    [SerializeField] private string name;
    [SerializeField] private Stage[] stages;
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> bonusLevels;

    public string GetName()
    {
        return this.name;
    }

    public Stage[] GetStages()
    {
        return this.stages;
    }

    public GameObject GetPlayer()
    {
        return this.player;
    }

    public List<GameObject> GetBonus()
    {
        return this.bonusLevels;
    }
}
