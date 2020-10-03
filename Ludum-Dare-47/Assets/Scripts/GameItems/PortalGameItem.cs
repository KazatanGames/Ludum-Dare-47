using UnityEngine;
using System.Collections;

public class PortalGameItem : TriggeringGameItem
{
    [SerializeField]
    protected Light portalLight;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameSceneManager.INSTANCE.TriggerPoint(TriggerData);
        }
    }

}
