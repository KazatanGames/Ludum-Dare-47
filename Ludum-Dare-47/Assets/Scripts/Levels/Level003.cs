using UnityEngine;
using System.Collections.Generic;

public class Level003 : BaseLevel
{
    protected bool crateRight = false;
    protected bool crateLeft = false;

    TriggeringGameItem leftPortal;
    TriggeringGameItem rightPortal;
    GameObject crate;

    Transform savedParent;

    public override void Create(Transform parent)
    {
        savedParent = parent;

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
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Inanimate, TriggerData.rightPortalNormal);

        crate = CreateSimpleLevelItem(
            LevelItemType.Crate,
            new Vector3(0, 10f, 0),
            Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward),
            parent
        );
    }

    protected override void TriggerActivated(TriggeringGameItem tgi, TriggerInteractionType type, ExtraTriggerData extras)
    {
        if (crateRight && crateLeft)
        {
            if (tgi == leftPortal && type == TriggerInteractionType.Portal_Player) shouldAdvance = true;
        }
        else if (crateLeft)
        {
            if (tgi == rightPortal && type == TriggerInteractionType.Portal_Inanimate && extras.colliderLevelItem.gameObject == crate)
            {
                crateRight = true;
                crate.SetActive(false);
                GameObject.Destroy(crate);
            }
        }
        else
        {
            if (tgi == leftPortal && type == TriggerInteractionType.Portal_Inanimate && extras.colliderLevelItem.gameObject == crate)
            {
                crateLeft = true;
                crate.SetActive(false);
                GameObject.Destroy(crate);
                crate = CreateSimpleLevelItem(
                    LevelItemType.Crate,
                    new Vector3(0, 10f, 0),
                    Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward),
                    savedParent
                );
            }
        }
    }
}
