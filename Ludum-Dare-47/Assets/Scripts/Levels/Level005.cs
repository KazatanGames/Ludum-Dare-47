using UnityEngine;
using System.Collections.Generic;

public class Level005 : BaseLevel
{
    protected bool ballLeft = false;

    TriggeringGameItem leftPortal;
    TriggeringGameItem rightPortal;
    GameObject ball;

    PlateGameItem plate;
    PlateGameItem plate2;

    Transform savedParent;

    public override void Create(Transform parent)
    {
        savedParent = parent;

        CreateSimpleLevelItem(
            LevelItemType.MediumPlatform,
            new Vector3(7f, 1.5f, 0),
            Quaternion.identity,
            parent
        );

        leftPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(-7f, 0, 0),
            Quaternion.identity,
            parent
        );
        TriggerData td = new TriggerData() { playerPositionOffset = new Vector3(13.1f, 3f, 0f), moveObject = true };
        AddTrigger(leftPortal, TriggerInteractionType.Portal_Player, td);
        AddTrigger(leftPortal, TriggerInteractionType.Portal_Inanimate, td);

        rightPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(7f, 3f, 0),
            Quaternion.AngleAxis(180, Vector3.up),
            parent
        );
        td = new TriggerData() { playerPositionOffset = new Vector3(-13.1f, -3f, 0f), moveObject = true };
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Player, td);
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Inanimate, td);

        Collider crate = CreateSimpleLevelItem(
            LevelItemType.Crate,
            new Vector3(2f, 8f, 0),
            Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward),
            parent
        ).GetComponent<Collider>();
        Collider crate2 = CreateSimpleLevelItem(
            LevelItemType.Crate,
            new Vector3(5f, 8f, 0),
            Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward),
            parent
        ).GetComponent<Collider>();

        plate = CreateSimpleLevelItem(
            LevelItemType.Plate,
            new Vector3(-2.2f, 0f, 0),
            Quaternion.identity,
            parent
        ).GetComponent<PlateGameItem>();

        plate2 = CreateSimpleLevelItem(
            LevelItemType.Plate,
            new Vector3(0f, 0f, 0),
            Quaternion.identity,
            parent
        ).GetComponent<PlateGameItem>();

        plate.SetGoodColliders(new List<Collider>() { crate, crate2 });
        plate2.SetGoodColliders(new List<Collider>() { crate, crate2 });
    }

    protected override void TriggerActivated(TriggeringGameItem tgi, TriggerInteractionType type, ExtraTriggerData extras)
    {
        if (plate.IsPlateGood && plate2.IsPlateGood)
        {
            if (tgi == rightPortal && type == TriggerInteractionType.Portal_Player) shouldAdvance = true;
        }
    }
}
