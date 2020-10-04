using UnityEngine;
using System.Collections.Generic;

public class LevelEnd : BaseLevel
{
    public override void Create(Transform parent)
    {
        //TriggeringGameItem leftPortal = CreateTGI(
        //    LevelItemType.Portal,
        //    new Vector3(-10, 0, 0),
        //    Quaternion.identity,
        //    parent
        //);
        //AddTrigger(leftPortal, TriggerInteractionType.Portal_Player, TriggerData.leftPortalNormal);
        //AddTrigger(leftPortal, TriggerInteractionType.Portal_Inanimate, TriggerData.leftPortalNormal);

        //TriggeringGameItem rightPortal = CreateTGI(
        //    LevelItemType.Portal,
        //    new Vector3(10, 0, 0),
        //    Quaternion.AngleAxis(180, Vector3.up),
        //    parent
        //);
        //AddTrigger(rightPortal, TriggerInteractionType.Portal_Player, TriggerData.rightPortalNormal);
        //AddTrigger(rightPortal, TriggerInteractionType.Portal_Inanimate, TriggerData.rightPortalNormal);

        CreateSimpleLevelItem(
            LevelItemType.Crate,
            new Vector3(0, 0.5f, 0),
            Quaternion.identity,
            parent
        );
        CreateSimpleLevelItem(
            LevelItemType.Crate,
            new Vector3(0.5f, 1.5f, 0),
            Quaternion.identity,
            parent
        );
        CreateSimpleLevelItem(
            LevelItemType.Crate,
            new Vector3(1, 0.5f, 0),
            Quaternion.identity,
            parent
        );
        CreateSimpleLevelItem(
            LevelItemType.Crate,
            new Vector3(2, 0.5f, 0),
            Quaternion.identity,
            parent
        );
    }

    protected override void TriggerActivated(TriggeringGameItem tgi, TriggerInteractionType type, ExtraTriggerData extras)
    {
        shouldAdvance = true;
    }
}
