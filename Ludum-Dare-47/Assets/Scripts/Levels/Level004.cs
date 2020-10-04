using UnityEngine;
using System.Collections.Generic;

public class Level004 : BaseLevel
{
    protected bool ballLeft = false;

    TriggeringGameItem leftPortal;
    TriggeringGameItem rightPortal;
    GameObject ball;

    Transform savedParent;

    public override void Create(Transform parent)
    {
        savedParent = parent;

        CreateSimpleLevelItem(
            LevelItemType.MediumPlatform,
            new Vector3(-7f, 1.5f, 0),
            Quaternion.identity,
            parent
        );

        leftPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(-7f, 3, 0),
            Quaternion.identity,
            parent
        );
        TriggerData td = new TriggerData() { playerPositionOffset = new Vector3(13.1f, -3f, 0f), moveObject = true };
        AddTrigger(leftPortal, TriggerInteractionType.Portal_Player, td);
        AddTrigger(leftPortal, TriggerInteractionType.Portal_Inanimate, td);

        rightPortal = CreateTGI(
            LevelItemType.Portal,
            new Vector3(7f, 0, 0),
            Quaternion.AngleAxis(180, Vector3.up),
            parent
        );
        td = new TriggerData() { playerPositionOffset = new Vector3(-13.1f, 3f, 0f), moveObject = true };
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Player, td);
        AddTrigger(rightPortal, TriggerInteractionType.Portal_Inanimate, td);

        ball = CreateSimpleLevelItem(
            LevelItemType.Ball,
            new Vector3(-4.5f, 3.5f, 0),
            Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward),
            parent
        );
    }

    protected override void TriggerActivated(TriggeringGameItem tgi, TriggerInteractionType type, ExtraTriggerData extras)
    {
        if (ballLeft)
        {
            if (tgi == rightPortal && type == TriggerInteractionType.Portal_Player) shouldAdvance = true;
        }
        else
        {
            if (tgi == leftPortal && type == TriggerInteractionType.Portal_Inanimate && extras.colliderLevelItem.gameObject == ball)
            {
                ballLeft = true;
                ParticlesManager.INSTANCE.DestroyCrateParticles(ball.transform.position);
                ball.SetActive(false);
                GameObject.Destroy(ball);
                PortalsAudio.INSTANCE.left2.Play();
            }
        }
    }
}
