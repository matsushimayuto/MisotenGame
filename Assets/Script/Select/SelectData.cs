using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectData", menuName = "Scriptable Objects/SelectData")]
public class SelectData : ScriptableObject
{
    public List<WorldData> worlds;
}

[System.Serializable]
public class StageData
{
    public int StageNumber;
    public bool isUnlocked;
}

[System.Serializable]
public class WorldData
{
    public int worldNumber;
    public List<StageData> stages;
}
