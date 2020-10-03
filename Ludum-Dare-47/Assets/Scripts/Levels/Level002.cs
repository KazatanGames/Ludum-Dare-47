using UnityEngine;
using System.Collections.Generic;

public class Level002 : BaseLevel
{
    protected bool crateRight = false;

    TriggeringGameItem leftPortal;
    TriggeringGameItem rightPortal;
    GameObject crate;

    public override void Create(Transform parent)
    {
        leftPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(-7f, 0, 0),
            Quaternion.identity,
            parent
        );
        AddTrigger(leftPortal, TriggerInteractionType.Portal_Player, TriggerData.leftPortalNormal);
        AddTrigger(leftPortal, TriggerInteractionType.Portal_Inanimate, TriggerData.leftPortalNormal);

        rightPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(7f, 0, 0),
            Quaternion.AngleAxis(180, Vector3.up),
            parent
        );
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Player, TriggerData.rightPortalNormal);

        crate = CreateSimpleLevelItem(
            LevelItemType.Crate,
            new Vector3(0, 0.5f, 0),
            Quaternion.identity,
            parent
        );
    }

    protected override void TriggerActivated(TriggeringGameItem tgi, TriggerInteractionType type, ExtraTriggerData extras)
    {
        if (crateRight)
        {
            if (tgi == leftPortal && type == TriggerInteractionType.Portal_Player) shouldAdvance = true;
        }
        else
        {
            if (tgi == rightPortal && type == TriggerInteractionType.Portal_Inanimate && extras.colliderLevelItem.gameObject == crate)
            {
                crateRight = true;
                GameObject.Destroy(crate);
            }
        }
    }
}
