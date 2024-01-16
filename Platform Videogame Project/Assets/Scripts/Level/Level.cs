using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Level
{
    [SerializeField] private string name;
    [SerializeField] private List<Stage> stages;
    [SerializeField] private List<GameObject> bonusLevels;

    public string GetName()
    {
        return this.name;
    }

    public List<Stage> GetStages()
    {
        return this.stages;
    }

    public List<GameObject> GetBonusLevels()
    {
        return this.bonusLevels;
    }
}
