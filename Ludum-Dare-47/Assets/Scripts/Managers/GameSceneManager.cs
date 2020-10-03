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
    protected float playerSpeed;
    [SerializeField]
    protected Vector3 initialPlayerPosition;
    [SerializeField]
    protected LevelItemPrefabMapSO prefabMap;

    // variables
    protected int currentLevel = -1;
    protected int maxLevel = 50;
    protected BaseLevel currentLevelData = new Level001();
    protected Transform levelContainer;
    protected bool velocityFlip = false;

    // managers
    protected InputManager inputManager;

    public void TriggerPoint(TriggerData tData)
    {
        if (tData.advanceLevel)
        {
            IncrementLevel();
        }

        playerRb.position += tData.playerPositionOffset;
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
        moonGameItem.SetPositionRatio((float)currentLevel / (float)maxLevel);

        DrawLevel();
    }

    protected void DrawLevel()
    {
        if (levelContainer != null)
        {
            Destroy(levelContainer.gameObject);
        }
        levelContainer = (new GameObject("Level Container")).transform;
        currentLevelData.Create(levelContainer);
    }

    protected override void Initialise()
    {
        inputManager = new InputManager();
        //playerRb.position = new Vector3(-8f, 0.01f, 0);
        moonGameItem.SetPositionRatio(currentLevel / maxLevel);

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

        if (inputs.hasHorizontal) { 
            velocity.x = playerSpeed * inputs.horizontal;
            playerRb.velocity = velocity;
        }
        inputManager.ResetInputs();
    }
}
