using UnityEngine;
using System.Collections.Generic;

public class PlayerGameItem : MonoBehaviour
{
    [SerializeField]
    protected Renderer frontRenderer;
    [SerializeField]
    protected Renderer leftRenderer;
    [SerializeField]
    protected Renderer rightRenderer;
    [SerializeField]
    protected AudioSource audioSource;
    [SerializeField]
    protected List<AudioClip> jumpAudioClips;

    public PlayerAnimType CurAnim { get; protected set; }

    protected int framesInTex = 8;
    protected float frameDuration = 1f;

    protected AnimInfoStruct idleFront = new AnimInfoStruct() { framesInAnim = 2, startFrame = 0, fps = 2 };
    protected AnimInfoStruct idleSide = new AnimInfoStruct() { framesInAnim = 2, startFrame = 2, fps = 3 };
    protected AnimInfoStruct walk = new AnimInfoStruct() { framesInAnim = 2, startFrame = 4, fps = 5 };
    protected AnimInfoStruct jumping = new AnimInfoStruct() { framesInAnim = 1, startFrame = 6, fps = 1 };

    protected float textOffset = 0f;
    protected int framesInAnim = 1;
    protected int currentFrame = 0;

    protected float frameTime = 0f;
    protected float frameWidth;

    protected bool flipped = false;

    protected Material frontMaterial;

    public void ChangeAnim(PlayerAnimType type)
    {
        if (CurAnim == type) return;

        switch(type)
        {
            case PlayerAnimType.Idle_Front:
                SetAnimInfo(idleFront);
                break;
            case PlayerAnimType.Walk:
                SetAnimInfo(walk);
                break;
            case PlayerAnimType.Jump:
                SetAnimInfo(jumping);
                break;
            case PlayerAnimType.Idle_Side:
            default:
                SetAnimInfo(idleSide);
                break;
        }

        CurAnim = type;

        frameTime = 0;
        currentFrame = 0;

        DrawFrame();
    }

    public void PlayAudioJump()
    {
        audioSource.clip = jumpAudioClips[Random.Range(0, jumpAudioClips.Count)];
        audioSource.Play();
    }

    public bool IsIdle => CurAnim == PlayerAnimType.Idle_Front || CurAnim == PlayerAnimType.Idle_Side;

    public bool IsFlipped => flipped;

    public void SetFlip(bool flipped)
    {
        if (flipped == this.flipped) return;
        this.flipped = flipped;

        frontMaterial.SetTextureScale("_MainTex", new Vector2(flipped ? -1 : 1, frameWidth));
        DrawFrame();
    }

    protected void SetAnimInfo(AnimInfoStruct ais)
    {
        textOffset = ais.startFrame * frameWidth;
        framesInAnim = ais.framesInAnim;
        frameDuration = 1f / ais.fps;
    }

    protected void Awake()
    {
        frameWidth = 1f / framesInTex;

        frontMaterial = frontRenderer.GetComponent<Renderer>().material;

        ChangeAnim(PlayerAnimType.Idle_Side);
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
        frontMaterial.mainTextureOffset = new Vector2(flipped ? 1 : 0, textOffset + (currentFrame * frameWidth));
    }
}
