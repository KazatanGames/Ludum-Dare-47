using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level Data X", menuName = "LD47/Level Data", order = 1)]
public class LevelDataSO : ScriptableObject
{
    [SerializeField]
    public List<string> requiredTriggers;
}
