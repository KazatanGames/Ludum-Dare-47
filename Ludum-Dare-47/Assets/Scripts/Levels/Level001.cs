using UnityEngine;
using System.Collections.Generic;

public class Level001 : BaseLevel
{
    public override void Create(Transform parent)
    {
        TriggeringGameItem leftPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(-7, 0, 0),
            Quaternion.identity,
            parent
        );
        AddTrigger(leftPortal, TriggerInteractionType.Portal_Player, TriggerData.leftPortalAdvance);

        TriggeringGameItem rightPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(7, 0, 0),
            Quaternion.AngleAxis(180, Vector3.up),
            parent
        );
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Player, TriggerData.rightPortalNormal);
    }
}
