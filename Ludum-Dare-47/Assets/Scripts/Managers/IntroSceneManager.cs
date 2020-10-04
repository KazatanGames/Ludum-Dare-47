using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSceneManager : MonoBehaviour
{

    [Header("Main Setup")]
    [SerializeField]
    protected List<IntroStateTimingStruct> playlist;
    [SerializeField]
    protected Utilities.SceneField nextScene;
    [SerializeField]
    protected AudioSource roomAudioSource;

    [Header("Fade In")]
    [SerializeField]
    protected RawImage blacknessImg;
    [SerializeField]
    protected GameObject titleGameObject;
    [SerializeField]
    protected float titleStartY;
    [SerializeField]
    protected float titleEndY;
    [SerializeField]
    protected float titleAnimDuration;
    [SerializeField]
    protected float titleAnimDelay;

    [SerializeField]
    protected AudioClip sleepAudioClip;
    [SerializeField]
    protected AudioClip surpriseAudioClip;
    [SerializeField]
    protected AudioClip curtainsAudioClip;
    [SerializeField]
    protected AudioClip curtainsShutAudioClip;
    [SerializeField]
    protected AudioClip doorAudioClip;

    [Header("Portal Appear")]
    [SerializeField]
    protected GameObject portalPrefab;

    [Header("House Action")]
    [SerializeField]
    protected Texture houseTexture;
    [SerializeField]
    protected Texture windowTexture;
    [SerializeField]
    protected Texture doorTexture;
    [SerializeField]
    protected Renderer houseRenderer;
    [SerializeField]
    protected GameObject doorGameObject;
    [SerializeField]
    protected GameObject upstairsLight;

    [Header("Play Game")]
    [SerializeField]
    protected PlayerGameItem player;
    [SerializeField]
    protected GameObject keysGameObject;
    [SerializeField]
    protected Transform playerFrontRendererTransform;
    [SerializeField]
    protected float playerMaxSpeed;
    [SerializeField]
    protected float playerAccel;
    [SerializeField]
    protected float jumpImpulse;
    [SerializeField]
    protected float keyFlipTime;
    [SerializeField]
    protected List<Material> keyFlipMaterials;

    protected int currentIntroStateIndex;
    protected IntroStateTimingStruct currentIntroState;
    protected float delayTimeRemaining;
    protected float stateTimeRemaining;
    protected bool stateLogicComplete;

    protected float totalTimeInState;
    protected float timingTimeInState;
    protected bool isDelay;
    protected bool done;

    protected bool sleepAudioPlayed = false;
    protected bool portalAppeared = false;
    protected bool windowOpen = false;
    protected bool doorOpen = false;

    protected bool isJumping = false;
    protected float horizontalInputSinceFixedUpdate = 0f;
    protected bool shouldJump = false;
    protected float timeIdle = 0f;
    protected float nextKeyFlip = 99f;
    protected int keyFlipIndex = 0;
    protected float titleAnimTime = 0f;

    protected PortalGameItem portalGI;

    protected Rigidbody playerRb;

    protected void Awake()
    {
        done = false;
        ChangeState(0);
    }

    private void Update()
    {
        if (done) return;

        if (titleAnimTime < titleAnimDuration + titleAnimDelay)
        {
            titleAnimTime += Time.deltaTime;
            if (titleAnimTime - titleAnimDelay >= 0)
            {
                titleGameObject.transform.position = new Vector3(
                    0,
                    Mathf.Lerp(titleStartY, titleEndY, Easing.Cubic.In((titleAnimTime - titleAnimDelay) / titleAnimDuration)),
                    0
                );
            }
        }

        CheckStateLogic();
    }

    protected void NextState()
    {
        ChangeState(currentIntroStateIndex + 1);
    }

    protected void ChangeState(int stateIndex)
    {
        currentIntroStateIndex = stateIndex;
        currentIntroState = playlist[currentIntroStateIndex];
        isDelay = true;
        totalTimeInState = 0;
        timingTimeInState = 0;

        switch (currentIntroState.state)
        {
            case IntroState.Fade_In_0:
                CreateFadeState();
                break;
            case IntroState.Portal_Appear:
                break;
            case IntroState.Open_Window:
                break;
            case IntroState.Open_Door:
                break;
            case IntroState.Player_Ready:
                nextKeyFlip = keyFlipTime;
                break;
            case IntroState.Done:
                AppManager.INSTANCE.AppModel.playerY = playerRb.position.y;
                AppManager.INSTANCE.AppModel.playerVelocity = playerRb.velocity;
                break;
        }
    }

    protected void CheckStateLogic()
    {
        totalTimeInState += Time.deltaTime;
        if (totalTimeInState >= currentIntroState.delay) isDelay = false;

        if (!isDelay)
        {
            timingTimeInState += Time.deltaTime;
        }

        switch (currentIntroState.state)
        {
            case IntroState.Fade_In_0:
                FadeInStateLogic();
                break;
            case IntroState.Portal_Appear:
                PortalAppearStateLogic();
                break;
            case IntroState.Open_Window:
                WindowStateLogic();
                break;
            case IntroState.Open_Door:
                DoorStateLogic();
                break;
            case IntroState.Player_Ready:
                PlayStateLogic();
                break;
            case IntroState.Done:
                if (totalTimeInState > 0.5f)
                {
                    done = true;
                    SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
                }
                break;
        }
    }

    protected void CreateFadeState()
    {
        blacknessImg.enabled = true;
        Color curCol = blacknessImg.color;
        curCol.a = 1;
        blacknessImg.color = curCol;

        MiniMusicManager.INSTANCE.Go();
    }

    protected void FlipKeys(int index)
    {
        if (index >= keyFlipMaterials.Count) index = 0;

        keyFlipIndex = index;

        keysGameObject.GetComponent<Renderer>().material = keyFlipMaterials[keyFlipIndex];
    }

    protected void FadeInStateLogic()
    {
        if (timingTimeInState >= currentIntroState.timing)
        {
            NextState();
            return;
        }

        if (timingTimeInState >= currentIntroState.timing / 4 && !sleepAudioPlayed)
        {
            roomAudioSource.PlayOneShot(sleepAudioClip);
            sleepAudioPlayed = true;
        }

        Color curCol = blacknessImg.color;
        float ratio = 1f - timingTimeInState / currentIntroState.timing;
        curCol.a = Easing.Quadratic.InOut(ratio);
        blacknessImg.color = curCol;
    }

    protected void PortalAppearStateLogic()
    {
        if (isDelay) return;
        if (portalAppeared)
        {
            if (timingTimeInState >= currentIntroState.timing)
            {
                NextState();
                return;
            }
        } else
        {
            portalGI = GameObject.Instantiate(
                portalPrefab,
                new Vector3(7f, 0, 0),
                Quaternion.AngleAxis(180, Vector3.up)
            ).GetComponent<PortalGameItem>();

            portalAppeared = true;

            roomAudioSource.Stop();
            roomAudioSource.clip = surpriseAudioClip;
            roomAudioSource.Play();
        }
    }

    protected void WindowStateLogic()
    {
        if (isDelay && !windowOpen)
        {
            windowOpen = true;
            houseRenderer.material.SetTexture("_MainTex", windowTexture);
            houseRenderer.material.SetTexture("_EmissionMap", windowTexture);
            upstairsLight.SetActive(true);

            roomAudioSource.Stop();
            roomAudioSource.clip = curtainsAudioClip;
            roomAudioSource.Play();
        }

        if (isDelay) return;

        if (windowOpen)
        {
            windowOpen = false;
            houseRenderer.material.SetTexture("_MainTex", houseTexture);
            houseRenderer.material.SetTexture("_EmissionMap", houseTexture);
            upstairsLight.SetActive(false);
            roomAudioSource.Stop();
            roomAudioSource.clip = curtainsShutAudioClip;
            roomAudioSource.Play();
        }

        if (timingTimeInState >= currentIntroState.timing)
        {
            NextState();
            return;
        }
    }

    protected void DoorStateLogic()
    {
        if (isDelay) return;

        if (!doorOpen)
        {
            doorOpen = true;
            houseRenderer.material.SetTexture("_MainTex", doorTexture);
            houseRenderer.material.SetTexture("_EmissionMap", doorTexture);

            doorGameObject.SetActive(true);

            roomAudioSource.Stop();
            roomAudioSource.clip = doorAudioClip;
            roomAudioSource.Play();
        }

        if (timingTimeInState >= currentIntroState.timing)
        {
            player.gameObject.SetActive(true);

            playerRb = player.GetComponent<Rigidbody>();

            player.SetFlip(true);
            player.ChangeAnim(PlayerAnimType.Walk);

            playerRb.AddForce(Vector3.right * 5f, ForceMode.Impulse);

            keysGameObject.SetActive(true);

            FlipKeys(0);

            NextState();
            return;
        }
    }

    protected void PlayStateLogic()
    {
        nextKeyFlip -= Time.deltaTime;
        if (nextKeyFlip <= 0)
        {
            nextKeyFlip += keyFlipTime;
            FlipKeys(keyFlipIndex + 1);
        }


        bool input = (Input.GetAxis("Horizontal") + Input.GetAxis("Jump")) > 0.1f;

        if (playerRb.velocity.sqrMagnitude < 0.05f && !input && !isJumping)
        {
            // player is idle
            timeIdle += Time.deltaTime;
        }
        else
        {
            timeIdle = 0f;
        }

        if (isJumping)
        {
            if (playerRb.velocity.y > 0)
            {
                playerFrontRendererTransform.localRotation = Quaternion.Euler(0f, 0f, player.IsFlipped ? 25f : -25f);
            }
            else
            {
                playerFrontRendererTransform.localRotation = Quaternion.Euler(0f, 0f, player.IsFlipped ? -25f : 25f);
            }
        }

        horizontalInputSinceFixedUpdate = Input.GetAxis("Horizontal");
        if (Input.GetAxis("Jump") > 0.25f) shouldJump = true;

        UpdatePlayerAnim();
    }

    protected void UpdatePlayerAnim()
    {
        if (timeIdle > 0)
        {
            if (!player.IsIdle)
            {
                player.ChangeAnim(PlayerAnimType.Idle_Side);
            }
            else if (player.CurAnim == PlayerAnimType.Idle_Side && timeIdle > 2f)
            {
                player.ChangeAnim(PlayerAnimType.Idle_Front);
            }
        }
        else
        {
            if (isJumping)
            {
                if (player.CurAnim != PlayerAnimType.Jump)
                {
                    player.ChangeAnim(PlayerAnimType.Jump);
                }
            }
            else
            {
                if (player.CurAnim != PlayerAnimType.Walk)
                {
                    player.ChangeAnim(PlayerAnimType.Walk);
                }
            }

            if (horizontalInputSinceFixedUpdate > 0.05f)
            {
                player.SetFlip(true);
            }
            else if (horizontalInputSinceFixedUpdate < -0.05f)
            {
                player.SetFlip(false);
            }
        }
    }

    protected void FixedUpdate()
    {
        if (currentIntroState.state != IntroState.Player_Ready || done) return;

        if (playerRb.position.x >= 6.75f)
        {
            portalGI.PlayAudioZap();

            NextState();
            playerRb.isKinematic = true;
            player.gameObject.SetActive(false);

            return;
        }

        // check for landing
        if (Mathf.Abs(playerRb.velocity.y) < 0.1f)
        {
            if (isJumping)
            {
                float distance = 0.4f;
                if (Physics.Raycast(playerRb.position + new Vector3(0, 0.1f, 0), Vector3.down, out _, distance))
                {
                    isJumping = false;
                    playerFrontRendererTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
        }
        else
        {
            isJumping = true;
        }

        // horizontal movement
        if (horizontalInputSinceFixedUpdate != 0f)
        {
            playerRb.AddForce(Vector3.right * horizontalInputSinceFixedUpdate * playerAccel, ForceMode.VelocityChange);
        }
        // ensure max speed
        Vector3 velocity = playerRb.velocity;
        velocity.x = Mathf.Clamp(velocity.x, -playerMaxSpeed, playerMaxSpeed);
        playerRb.velocity = velocity;

        // vertical movement
        if (shouldJump && !isJumping)
        {
            isJumping = true;
            playerRb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);

            player.PlayAudioJump();
        }

        // reset inputs
        horizontalInputSinceFixedUpdate = 0f;
        shouldJump = false;
    }
}
