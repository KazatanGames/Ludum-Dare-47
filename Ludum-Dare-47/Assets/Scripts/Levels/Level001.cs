using UnityEngine;
using System.Collections;

public class Level001 : BaseLevel
{
    public override void Create(Transform parent)
    {
        GameObject go = GameObject.Instantiate(
            GameSceneManager.INSTANCE.GetPrefab(LevelItemType.Portal),
            new Vector3(-10, 0, 0),
            Quaternion.identity,
            parent
        );
        TriggeringGameItem tgi = go.GetComponent<TriggeringGameItem>();
        tgi.TriggerData = new TriggerData()
        {
            playerPositionOffset = new Vector3(18.9f, 0f, 0f),
            advanceLevel = true
        };

        go = GameObject.Instantiate(
            GameSceneManager.INSTANCE.GetPrefab(LevelItemType.Portal),
            new Vector3(10, 0, 0),
            Quaternion.AngleAxis(180, Vector3.up),
            parent
        );
        tgi = go.GetComponent<TriggeringGameItem>();
        tgi.TriggerData = new TriggerData()
        {
            playerPositionOffset = new Vector3(-18.9f, 0f, 0f),
            advanceLevel = false,
        };
    }
}
