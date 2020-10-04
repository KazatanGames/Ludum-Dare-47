using UnityEngine;
using System.Collections;

public class PlateColliderGameItem : MonoBehaviour
{

    [SerializeField]
    protected PlateGameItem parentPlate;

    private void OnTriggerEnter(Collider other)
    {
        parentPlate.TriggerEnterPlate(other);
    }

    private void OnTriggerExit(Collider other)
    {
        parentPlate.TriggerExitPlate(other);
    }
}
