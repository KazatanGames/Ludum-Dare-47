using UnityEngine;
using System.Collections;

public class PortalGameItem : TriggeringGameItem
{
    [SerializeField]
    protected Light portalLight;
    [SerializeField]
    protected float moveDistance = 0.15f;
    [SerializeField]
    protected Renderer frontRenderer;

    protected float frameDuration = 1f / 8;

    protected int framesInAnim = 3;
    protected int currentFrame = 0;

    protected float frameTime = 0f;
    protected float frameWidth = 1f / 4;

    protected Material frontMaterial;

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

    protected void Awake()
    {
        frontMaterial = frontRenderer.GetComponent<Renderer>().material;
    }

    protected void Start()
    {
        portalLight.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        portalLight.transform.Translate(Vector3.forward * moveDistance, Space.Self);
    }

    protected void Update()
    {
        frameTime += Time.deltaTime;

        if (frameTime > frameDuration)
        {
            frameTime -= frameDuration;
            NextFrame();
        }
    }

    protected void NextFrame()
    {
        currentFrame++;
        if (currentFrame >= framesInAnim) currentFrame = 0;
        DrawFrame();
    }

    protected void DrawFrame()
    {
        frontMaterial.mainTextureOffset = new Vector2(0, currentFrame * frameWidth);
    }

}
