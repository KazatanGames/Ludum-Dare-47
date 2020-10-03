using UnityEngine;
using System.Collections.Generic;

public class Level002 : BaseLevel
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
        AddTrigger(leftPortal, TriggerInteractionType.Portal_Inanimate, TriggerData.leftPortalNormal);

        TriggeringGameItem rightPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(7, 0, 0),
            Quaternion.AngleAxis(180, Vector3.up),
            parent
        );
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Player, TriggerData.rightPortalNormal);
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Inanimate, TriggerData.rightPortalNormal);

        TriggeringGameItem crate = CreateTGI(
            LevelItemType.Crate,
            new Vector3(0, 0.5f, 0),
            Quaternion.identity,
            parent
        );
    }
}
