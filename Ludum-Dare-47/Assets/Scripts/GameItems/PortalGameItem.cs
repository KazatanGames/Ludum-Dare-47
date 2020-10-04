using UnityEngine;
using System.Collections.Generic;

public class PortalGameItem : TriggeringGameItem
{
    [SerializeField]
    protected Light portalLight;
    [SerializeField]
    protected float moveDistance = 0.15f;
    [SerializeField]
    protected Renderer frontRenderer;
    [SerializeField]
    protected List<AudioClip> zapNoises;

    protected float frameDuration = 1f / 16;

    protected int framesInAnim = 4;
    protected int currentFrame = 0;

    protected float frameTime = 0f;
    protected float frameWidth = 1f / 4;

    protected Material frontMaterial;

    public void OnTriggerEnter(Collider other)
    {
        if (level == null) return;

        if (other.tag == "Player")
        {
            PlayAudioZap();
            level.Trigger(this, TriggerInteractionType.Portal_Player, new ExtraTriggerData() { objectSize = GameConsts.playerBounds.size, colliderLevelItem = other });
        }
        else if (other.tag == "Inanimate")
        {
            PlayAudioZap();

            Renderer mr = other.gameObject.GetComponent<Renderer>();

            level.Trigger(this, TriggerInteractionType.Portal_Inanimate, new ExtraTriggerData() { objectSize = mr.bounds.size, colliderLevelItem = other });
        }
    }

    public void PlayAudioZap()
    {
        AudioSource audioSource;
        if (transform.position.x >= 5)
        {
            audioSource = PortalsAudio.INSTANCE.right;
        } else if (transform.position.x <= -5) {
            audioSource = PortalsAudio.INSTANCE.left;
        } else
        {
            audioSource = PortalsAudio.INSTANCE.center;
        }

        audioSource.clip = zapNoises[Random.Range(0, zapNoises.Count)];
        audioSource.Play();
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
