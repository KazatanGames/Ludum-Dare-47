﻿using UnityEngine;
using System.Collections;

public class PortalGameItem : TriggeringGameItem
{
    [SerializeField]
    protected Light portalLight;

    protected float moveDistance = 0.15f;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            level.Trigger(this, TriggerInteractionType.Portal_Player, new ExtraTriggerData() { objectSize = GameConsts.playerBounds.size, colliderLevelItem = other });
        }
        else if (other.tag == "Inanimate")
        {
            Renderer mr = other.gameObject.GetComponent<Renderer>();

            level.Trigger(this, TriggerInteractionType.Portal_Inanimate, new ExtraTriggerData() { objectSize = mr.bounds.size, colliderLevelItem = other });
        }
    }

    protected void Start()
    {
        portalLight.transform.Translate(new Vector3(0, 0, -1) * moveDistance, Space.Self);
    }

}
