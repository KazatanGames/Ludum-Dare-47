using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Game Data", menuName = "LD47/Game Data", order = 2)]
public class GameDataSO : ScriptableObject
{
    [SerializeField]
    public List<LevelDataSO> levelDatas;
}
