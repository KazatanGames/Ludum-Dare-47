using UnityEngine;
using System.Collections.Generic;

public class Level001 : BaseLevel
{
    TriggeringGameItem leftPortal;

    public override void Create(Transform parent)
    {
        leftPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(-7f, 0, 0),
            Quaternion.identity,
            parent
        );
        AddTrigger(leftPortal, TriggerInteractionType.Portal_Player, TriggerData.leftPortalNormal);

        TriggeringGameItem rightPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(7f, 0, 0),
            Quaternion.AngleAxis(180, Vector3.up),
            parent
        );
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Player, TriggerData.rightPortalNormal);
    }

    protected override void TriggerActivated(TriggeringGameItem tgi, TriggerInteractionType type, ExtraTriggerData extras)
    {
        if (tgi == leftPortal && type == TriggerInteractionType.Portal_Player) shouldAdvance = true;
    }
}
