using UnityEngine;
using System.Collections.Generic;

public class GameSceneManager : SingletonMonoBehaviour<GameSceneManager>
{
    [SerializeField]
    protected Rigidbody playerRb;
    [SerializeField]
    protected MoonGameItem moonGameItem;
    [SerializeField]
    protected PlayerGameItem playerGameItem;
    [SerializeField]
    protected Transform playerFrontRendererTransform;
    [Header("Game Setup")]
    [SerializeField]
    protected float playerMaxSpeed;
    [SerializeField]
    protected float playerAccel;
    [SerializeField]
    protected float jumpImpulse;
    [SerializeField]
    protected Vector3 initialPlayerPosition;
    [SerializeField]
    protected LevelItemPrefabMapSO prefabMap;

    [Header("Testing")]
    protected int startingLevel;

    // variables
    protected int currentLevel = 0;
    protected Transform levelContainer;
    protected bool isJumping = false;

    protected float horizontalInputSinceFixedUpdate = 0f;
    protected bool shouldJump = false;

    protected float timeIdle = 0f;

    [Header("End Titles")]
    [SerializeField]
    protected GameObject titleGameObject;
    protected bool finished = false;
    [SerializeField]
    protected float titleStartY;
    [SerializeField]
    protected float titleEndY;
    [SerializeField]
    protected float titleAnimDuration;

    protected float titleAnimTime = 0f;

    // levels
    protected List<BaseLevel> levels = new List<BaseLevel>()
    {
        new Level001(),
        new Level002(),
        new Level003(),
        new LevelEnd(),
    };

    protected Dictionary<Rigidbody, Vector3> positionChanges = new Dictionary<Rigidbody, Vector3>();

    // managers
    protected InputManager inputManager;

    public void TriggerPoint(bool advanceLevel, TriggerData tData, ExtraTriggerData extras)
    {
        if (advanceLevel)
        {
            IncrementLevel();
        }

        if (tData.moveObject && extras.colliderLevelItem.attachedRigidbody != null)
        {
            Vector3 dPos = tData.playerPositionOffset;

            if (tData.playerPositionOffset.x > 0)
            {
                dPos -= new Vector3(extras.objectSize.x / 2f, 0, 0);
            }
            else if (tData.playerPositionOffset.x < 0)
            {
                dPos += new Vector3(extras.objectSize.x / 2f, 0, 0);
            }

            if (positionChanges.ContainsKey(extras.colliderLevelItem.attachedRigidbody))
            {
                positionChanges[extras.colliderLevelItem.attachedRigidbody] = dPos;
            }
            else
            {
                positionChanges.Add(extras.colliderLevelItem.attachedRigidbody, dPos);
            }
        }
    }

    public GameObject GetPrefab(LevelItemType type)
    {
        foreach(LevelItemPrefabStruct lips in prefabMap.lookup)
        {
            if (lips.type == type) return lips.prefab;
        }
        return null;
    }

    protected void IncrementLevel()
    {
        currentLevel++;

        if (currentLevel == levels.Count - 1)
        {
            finished = true;
            titleGameObject.SetActive(true);
        }
        
        UpdateMoonPosition();
        DrawLevel();
    }

    protected void UpdateMoonPosition()
    {
        moonGameItem.SetPositionRatio((float)currentLevel / ((float)levels.Count - 1));
    }

    protected void DrawLevel()
    {
        if (levelContainer != null)
        {
            Destroy(levelContainer.gameObject);
        }
        levelContainer = (new GameObject("Level Container")).transform;

        levels[currentLevel].Create(levelContainer);
    }

    protected override void Initialise()
    {
        inputManager = new InputManager();
        currentLevel = startingLevel;
        UpdateMoonPosition();
        DrawLevel();

        playerGameItem.SetFlip(true);
        playerRb.velocity = AppManager.INSTANCE.AppModel.playerVelocity;
        playerRb.position = new Vector3(-6f, AppManager.INSTANCE.AppModel.playerY, playerRb.position.z);
        isJumping = Mathf.Abs(playerRb.velocity.y) < 0.1f;
        UpdatePlayerAnim();
    }

    protected void Update()
    {
        bool input = (Input.GetAxis("Horizontal") + Input.GetAxis("Jump")) > 0.1f;

        if (playerRb.velocity.sqrMagnitude < 0.05f && !input && !isJumping)
        {
            // player is idle
            timeIdle += Time.deltaTime;
        } else
        {
            timeIdle = 0f;
        }

        if (isJumping)
        {
            if (playerRb.velocity.y > 0)
            {
                playerFrontRendererTransform.localRotation = Quaternion.Euler(0f, 0f, playerGameItem.IsFlipped ? 25f : -25f);
            } else
            {
                playerFrontRendererTransform.localRotation = Quaternion.Euler(0f, 0f, playerGameItem.IsFlipped ? -25f : 25f);
            }
        }

        horizontalInputSinceFixedUpdate = Input.GetAxis("Horizontal");
        if (Input.GetAxis("Jump") > 0.25f) shouldJump = true;

        UpdatePlayerAnim();

        if (finished && titleAnimTime < titleAnimDuration)
        {
            titleAnimTime += Time.deltaTime;
            titleAnimTime = Mathf.Clamp(titleAnimTime, 0, titleAnimDuration);
            titleGameObject.transform.position = new Vector3(
                0,
                Mathf.Lerp(titleStartY, titleEndY, Easing.Cubic.Out(titleAnimTime / titleAnimDuration)),
                0
            );
        }
    }

    protected void UpdatePlayerAnim()
    {
        if (timeIdle > 0)
        {
            if (!playerGameItem.IsIdle)
            {
                playerGameItem.ChangeAnim(PlayerAnimType.Idle_Side);
            }
            else if (playerGameItem.CurAnim == PlayerAnimType.Idle_Side && timeIdle > 2f)
            {
                playerGameItem.ChangeAnim(PlayerAnimType.Idle_Front);
            }
        } else
        {
            if (isJumping)
            {
                if (playerGameItem.CurAnim != PlayerAnimType.Jump)
                {
                    playerGameItem.ChangeAnim(PlayerAnimType.Jump);
                }
            } else
            {
                if (playerGameItem.CurAnim != PlayerAnimType.Walk)
                {
                    playerGameItem.ChangeAnim(PlayerAnimType.Walk);
                }
            }

            if (horizontalInputSinceFixedUpdate > 0.05f)
            {
                playerGameItem.SetFlip(true);
            } else if (horizontalInputSinceFixedUpdate < -0.05f)
            {
                playerGameItem.SetFlip(false);
            }
        }
    }

    protected void FixedUpdate()
    {
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
        } else
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

            playerGameItem.PlayAudioJump();
        }

        // reset inputs
        horizontalInputSinceFixedUpdate = 0f;
        shouldJump = false;

        // position changes from triggered events
        foreach(KeyValuePair<Rigidbody, Vector3> keyValuePair in positionChanges)
        {
            keyValuePair.Key.position += keyValuePair.Value;
        }
        positionChanges.Clear();
    }
}
