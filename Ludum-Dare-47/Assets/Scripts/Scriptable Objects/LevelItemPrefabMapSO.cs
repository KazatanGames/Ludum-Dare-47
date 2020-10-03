using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelItem Prefab Map", menuName = "LD47/LevelItem Prefab Map", order = 3)]
public class LevelItemPrefabMapSO : ScriptableObject
{
    [SerializeField]
    public List<LevelItemPrefabStruct> lookup;
}
