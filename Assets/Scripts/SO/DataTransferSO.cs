using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DataTransferSO : ScriptableObject
{
    public int totalTime;
    public int level;
    public List<int> abilityLevels = new List<int>(){0, 0, 0, 0, 0, 0};
}
