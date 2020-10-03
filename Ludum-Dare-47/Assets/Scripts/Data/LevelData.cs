using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelData
{
    public delegate void OnActionTrigger(string itemId);

    protected Dictionary<string, LevelItemType> levelItems;
    protected Dictionary<string, OnActionTrigger> actionMap;
}
