using UnityEngine;
using System.Collections;

public class TriggerData
{
    public static TriggerData leftPortalAdvance = new TriggerData() { advanceLevel = true, playerPositionOffset = new Vector3(19.1f, 0f, 0f), moveObject = true };
    public static TriggerData leftPortalNormal = new TriggerData() { advanceLevel = false, playerPositionOffset = new Vector3(19.1f, 0f, 0f), moveObject = true };
    public static TriggerData rightPortalAdvance = new TriggerData() { advanceLevel = true, playerPositionOffset = new Vector3(-19.1f, 0f, 0f), moveObject = true };
    public static TriggerData rightPortalNormal = new TriggerData() { advanceLevel = false, playerPositionOffset = new Vector3(-19.1f, 0f, 0f), moveObject = true };


    // advance a level
    public bool advanceLevel;

    // move the object
    public bool moveObject;
    public Vector3 playerPositionOffset = Vector3.zero;
}
