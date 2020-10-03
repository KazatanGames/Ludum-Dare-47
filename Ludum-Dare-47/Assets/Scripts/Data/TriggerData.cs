using UnityEngine;
using System.Collections;

public class TriggerData
{
    public static TriggerData leftPortalNormal = new TriggerData() { playerPositionOffset = new Vector3(13.1f, 0f, 0f), moveObject = true };
    public static TriggerData rightPortalNormal = new TriggerData() { playerPositionOffset = new Vector3(-13.1f, 0f, 0f), moveObject = true };

    // move the object
    public bool moveObject;
    public Vector3 playerPositionOffset = Vector3.zero;
}
