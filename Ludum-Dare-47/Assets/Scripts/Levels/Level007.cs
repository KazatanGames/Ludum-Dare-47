using UnityEngine;
using System.Collections.Generic;

public class Level006 : BaseLevel
{
    protected bool crateRight = false;
    protected bool ballLeft = false;

    TriggeringGameItem leftPortal;
    TriggeringGameItem rightPortal;
    GameObject ball;
    GameObject crate;

    PlateGameItem plate;

    public override void Create(Transform parent)
    {
        leftPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(-7f, 0, 0),
            Quaternion.identity,
            parent
        );
        TriggerData td = new TriggerData() { playerPositionOffset = new Vector3(13.1f, 0f, 0f), moveObject = true };
        AddTrigger(leftPortal, TriggerInteractionType.Portal_Player, td);
        AddTrigger(leftPortal, TriggerInteractionType.Portal_Inanimate, td);

        rightPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(7f, 0f, 0),
            Quaternion.AngleAxis(180, Vector3.up),
            parent
        );
        td = new TriggerData() { playerPositionOffset = new Vector3(-13.1f, 0f, 0f), moveObject = true };
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Player, td);
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Inanimate, td);

        crate = CreateSimpleLevelItem(
            LevelItemType.Crate,
            new Vector3(2f, 8f, 0),
            Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward),
            parent
        );

        ball = CreateSimpleLevelItem(
            LevelItemType.Ball,
            new Vector3(2f, 10f, 0),
            Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward),
            parent
        );

        plate = CreateSimpleLevelItem(
            LevelItemType.Plate,
            new Vector3(-2f, 0f, 0),
            Quaternion.identity,
            parent
        ).GetComponent<PlateGameItem>();

        plate.SetGoodColliders(new List<Collider>() { ball.GetComponent<Collider>() });
    }

    protected override void TriggerActivated(TriggeringGameItem tgi, TriggerInteractionType type, ExtraTriggerData extras)
    {
        if (ballLeft && crateRight)
        {
            if (tgi == rightPortal && type == TriggerInteractionType.Portal_Player) shouldAdvance = true;
        }
        else if (crateRight)
        {
            if (tgi == leftPortal && type == TriggerInteractionType.Portal_Inanimate && extras.colliderLevelItem.gameObject == ball)
            {
                ballLeft = true;
                ParticlesManager.INSTANCE.DestroyCrateParticles(ball.transform.position);
                ball.SetActive(false);
                GameObject.Destroy(ball);
            }
        }
        else if (plate.IsPlateGood)
        {
            if (tgi == rightPortal && type == TriggerInteractionType.Portal_Inanimate && extras.colliderLevelItem.gameObject == crate)
            {
                crateRight = true;
                ParticlesManager.INSTANCE.DestroyCrateParticles(crate.transform.position);
                crate.SetActive(false);
                GameObject.Destroy(crate);
            }
        }
    }
}
