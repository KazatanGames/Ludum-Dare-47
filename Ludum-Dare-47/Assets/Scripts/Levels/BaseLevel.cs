using UnityEngine;
using System.Collections.Generic;

public abstract class BaseLevel
{
    protected Dictionary<TriggeringGameItem, Dictionary<TriggerInteractionType, TriggerData>> triggers;

    protected bool shouldAdvance = false;

    public BaseLevel()
    {
        triggers = new Dictionary<TriggeringGameItem, Dictionary<TriggerInteractionType, TriggerData>>(); ;
    }

    public abstract void Create(Transform parent);

    public void Trigger(TriggeringGameItem tgi, TriggerInteractionType type, ExtraTriggerData extras)
    {
        TriggerActivated(tgi, type, extras);

        if (triggers.ContainsKey(tgi) && triggers[tgi].ContainsKey(type))
        {
            GameSceneManager.INSTANCE.TriggerPoint(shouldAdvance, triggers[tgi][type], extras);
        }
    }
    public void Trigger(TriggeringGameItem tgi, TriggerInteractionType type)
    {
        Trigger(tgi, type, new ExtraTriggerData());
    }

    protected TriggeringGameItem CreateTGI(LevelItemType type, Vector3 position, Quaternion rotation, Transform parent)
    {
        TriggeringGameItem tgi = GameObject.Instantiate(
            GameSceneManager.INSTANCE.GetPrefab(type),
            position,
            rotation,
            parent
        ).GetComponent<TriggeringGameItem>();

        if (tgi != null)
        {
            tgi.level = this;
        }

        return tgi;
    }

    protected GameObject CreateSimpleLevelItem(LevelItemType type, Vector3 position, Quaternion rotation, Transform parent)
    {
        return GameObject.Instantiate(
            GameSceneManager.INSTANCE.GetPrefab(type),
            position,
            rotation,
            parent
        );
    }

    protected void AddTrigger(TriggeringGameItem tgi, TriggerInteractionType type, TriggerData tData)
    {
        if (!triggers.ContainsKey(tgi))
        {
            triggers.Add(tgi, new Dictionary<TriggerInteractionType, TriggerData>());
        }

        if (triggers[tgi].ContainsKey(type))
        {
            triggers[tgi][type] = tData;
        } else
        {
            triggers[tgi].Add(type, tData);
        }
    }

    protected abstract void TriggerActivated(TriggeringGameItem tgi, TriggerInteractionType type, ExtraTriggerData extras);
}
