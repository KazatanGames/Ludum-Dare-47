using UnityEngine;
using System.Collections.Generic;

public class GameSceneManager : SingletonMonoBehaviour<GameSceneManager>
{
    [SerializeField]
    protected Rigidbody playerRb;
    [SerializeField]
    protected MoonGameItem moonGameItem;
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

    // levels
    protected List<BaseLevel> levels = new List<BaseLevel>()
    {
        new Level001(),
        new Level002(),
        new Level003(),
    };

    protected Dictionary<Rigidbody, Vector3> positionChanges = new Dictionary<Rigidbody, Vector3>();

    // managers
    protected InputManager inputManager;

    public void TriggerPoint(TriggerData tData, ExtraTriggerData extras)
    {
        if (tData.advanceLevel)
        {
            IncrementLevel();
        }

        if (tData.moveObject)
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
        UpdateMoonPosition();
        DrawLevel();
    }

    protected void UpdateMoonPosition()
    {
        moonGameItem.SetPositionRatio((float)currentLevel / (float)levels.Count);
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
        UpdateMoonPosition();

        currentLevel = startingLevel;
        UpdateMoonPosition();
        DrawLevel();
    }

    protected void Update()
    {
        inputManager.UpdateInputs();
    }

    protected void FixedUpdate()
    {
        InputStruct inputs = inputManager.GetInputs();
        Vector3 velocity = playerRb.velocity;

        if (isJumping)
        {
            if (Mathf.Abs(playerRb.velocity.y) < 0.05f)
            {
                float distance = 0.25f;
                if (Physics.Raycast(playerRb.position + new Vector3(0, 0.1f, 0), Vector3.down, out _, distance))
                {
                    isJumping = false;
                }
            }
        }
        
        if (inputs.hasHorizontal) {
            playerRb.AddForce(Vector3.right * inputs.horizontal * playerAccel, ForceMode.Acceleration);
            velocity.x = Mathf.Clamp(velocity.x, -playerMaxSpeed, playerMaxSpeed);
            playerRb.velocity = velocity;
        }
        if (inputs.hasVertical && !isJumping)
        {
            isJumping = true;
            playerRb.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
        }

        inputManager.ResetInputs();

        foreach(KeyValuePair<Rigidbody, Vector3> keyValuePair in positionChanges)
        {
            keyValuePair.Key.position += keyValuePair.Value;
        }
        positionChanges.Clear();
    }
}
