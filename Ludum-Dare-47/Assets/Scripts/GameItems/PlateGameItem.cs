using UnityEngine;
using System.Collections.Generic;

public class PlateGameItem : MonoBehaviour
{
    [SerializeField]
    protected Light plateLight;
    [SerializeField]
    protected Material badMaterial;
    [SerializeField]
    protected Material goodMaterial;
    [SerializeField]
    protected Renderer plateRenderer;
    [SerializeField]
    protected Material goodSideMaterial;
    [SerializeField]
    protected Material badSideMaterial;
    [SerializeField]
    protected List<Renderer> sideRenderers;
    [SerializeField]
    protected Color badLightColor;
    [SerializeField]
    protected Color goodLightColor;

    protected int goodCollisions = 0;
    protected List<Collider> acceptedColliders;

    private void Awake()
    {
        goodCollisions = 0;
        UpdateMat();
    }

    protected void UpdateMat()
    {
        plateRenderer.material = goodCollisions > 0 ? goodMaterial : badMaterial;
        plateLight.color = goodCollisions > 0 ? goodLightColor : badLightColor;

        Material sideMat = goodCollisions > 0 ? goodSideMaterial : badSideMaterial;
        foreach (Renderer r in sideRenderers) r.material = sideMat;
    }

    public void SetGoodColliders(List<Collider> colliders)
    {
        acceptedColliders = colliders;
    }

    public void TriggerEnterPlate(Collider other)
    {
        if (acceptedColliders.Contains(other))
        {
            goodCollisions++;
            UpdateMat();
        }
    }

    public void TriggerExitPlate(Collider other)
    {
        if (acceptedColliders.Contains(other))
        {
            goodCollisions--;
            UpdateMat();
        }
    }

    public bool IsPlateGood => goodCollisions > 0;
}
